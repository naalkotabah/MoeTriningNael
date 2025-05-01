using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Hubs;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Moe.Core.Null;
using Moe.Core.Translations;

namespace Moe.Core.Services;

public class SignalRNotificationsService : INotificationsService
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<MasterHub> _hub;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;

    public SignalRNotificationsService(MasterDbContext context, IMapper mapper, IHubContext<MasterHub> hub, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _hub = hub;
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    #region Send

    public async Task Send(NotificationForm form)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Create a new scope
                using var scope = _serviceProvider.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

                // Database Insertion
                var notifications = form.NotifiersIds.Select(e => new Notification()
                {
                    Type = form.Type,
                    Tags = form.Tags,
                    HelperIdType = form.HelperIdType,
                    
                    ActorId = form.ActorId,
                    NotifierId = e,
                    
                    Title = form.Title,
                    Content = form.Content,
                    HelperId = form.HelperId
                }).ToList();

                await scopedContext.AddRangeAsync(notifications);
                await scopedContext.SaveChangesAsync();

                // SignalR Notification Sending
                var groupedNotifiers = await scopedContext.Users
                    .Where(u => !u.IsDeleted && form.NotifiersIds.Contains(u.Id))
                    .GroupBy(u => u.Lang)
                    .ToListAsync();

                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MasterHub>>();
                foreach (var languageGroup in groupedNotifiers)
                {
                    var language = languageGroup.Key;
                    var title = Localizer.Translate(form.Title, language.ToString());
                    var content = Localizer.Translate(form.Content, language.ToString());

                    var notification = new NotificationDTOSimplified()
                    {
                        Title = title,
                        Content = content,
                        HelperId = form.HelperId
                    };

                    var userIds = languageGroup.Select(u => u.Id.ToString()).ToList();
                    await hub.Clients.Users(userIds).SendAsync("ReceiveNotification", notification);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error processing notification: {ex.Message}");
            }
        });
    }

    #endregion

    #region Read
    public async Task<Response<PagedList<NotificationDTO>>> GetAll(NotificationsFilter filter)
    {
        var lang = _httpContextAccessor.HttpContext.GetLang();
        var dtos = await _context.Notifications
            .WhereBaseFilter(filter)
            
            .Where(e => filter.ActorId == null || e.ActorId == filter.ActorId)
            .Where(e => filter.NotifierId == null || e.NotifierId == filter.NotifierId)
            
            .Where(e => filter.Type == null || e.Type == filter.Type)
            .Where(e => filter.Tag == null || e.Tags.Contains(filter.Tag.Value))
            .Where(e => filter.HelperIdType == null || e.HelperIdType == filter.HelperIdType)
            
            .Where(e => filter.IsSeen == null || e.IsSeen == filter.IsSeen)
            .Where(e => filter.IsOpened == null || e.IsOpened == filter.IsOpened)
            
            .Where(e => filter.HelperId == null || e.HelperId == filter.HelperId)
            
            .OrderByCreationDate()
            .ProjectTo<NotificationDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);
            
        dtos.Items = dtos.Items
            .Select(e =>
            {
                e.Title = Localizer.Translate(e.Title, lang);
                e.Content = Localizer.Translate(e.Content, lang);
                return e;
            }).ToList();

        return new Response<PagedList<NotificationDTO>>(dtos, null, 200);
    }

    public async Task<Response<int>> CountUnseenNotifications(Guid notifierId)
    {
        var count = await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsSeen)
            .CountAsync();
        return new Response<int>(count, null, 200);
    }
    #endregion

    #region Write
    public async Task SwitchSeen(Guid id)
    {
        var notification = await _context.Notifications.Where(e => e.Id == id && !e.IsDeleted).FirstOrDefaultAsync();
        if (notification == null)
            ErrResponseThrower.NotFound();

        notification.IsSeen = !notification.IsSeen;
        _context.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task SwitchOpened(Guid id)
    {
        var notification = await _context.Notifications.Where(e => e.Id == id && !e.IsDeleted).FirstOrDefaultAsync();
        if (notification == null)
            ErrResponseThrower.NotFound();

        notification.IsOpened = !notification.IsOpened;
        _context.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task SetAllSeen(Guid notifierId)
    {
        await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsSeen)
            .ExecuteUpdateAsync(e => e.SetProperty(ee => ee.IsSeen, true));
    }

    public async Task SetAllOpened(Guid notifierId)
    {
        await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsOpened)
            .ExecuteUpdateAsync(e => e.SetProperty(ee => ee.IsOpened, true));
    }

    public async Task EnsureNotifierOwnsNotification(Guid notifierId, Guid notificationId)
    {
        var valid = await _context.Notifications
            .AnyAsync(e => e.NotifierId == notifierId && e.Id == notificationId);
        if (!valid)
            ErrResponseThrower.BadRequest("This notification doesn't belong to you");
    }
    #endregion
}