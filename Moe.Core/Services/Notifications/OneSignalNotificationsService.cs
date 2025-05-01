#define STOP_REAL_NOTIFICATIONS_OFF

using System.Net.Mime;
using System.Text;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Moe.Core.Null;

namespace Moe.Core.Services;

public class OneSignalNotificationsService : INotificationsService
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;
    
    private readonly string _appId;
    private readonly string _appKey;
    
    private const string ONE_SIGNAL_PUSH_NOTIFICATIONS_URL = "https://onesignal.com/api/v1/notifications";
    
    public OneSignalNotificationsService(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _appId = ConfigProvider.config["OneSignal:AppId"];
        _appKey = ConfigProvider.config["OneSignal:AppKey"];
    }

    #region Sending a notification
    public async Task Send(NotificationForm form)
    {
        if (!form.ActorId.IsNullOrZeros())
            await _context.EnsureEntityExists<User>(form.ActorId.Value, "NOTIFICATION_ACTOR_NOT_FOUND");
        await _context.EnsureEntitiesIdsExists<User>(form.NotifiersIds, "NOTIFICATION_NOTIFIER_NOT_FOUND");

        await InsertNotificationsIntoDb(form);
        Task.Run(SendNotificationThroughOneSignal(form).GetAwaiter().GetResult);
    }

    private async Task InsertNotificationsIntoDb(NotificationForm form)
    {
        if (!form.ActorId.IsNullOrZeros())
        {
            var actorExists = await _context.Users.AnyAsync(e => e.Id == form.ActorId.Value && !e.IsDeleted);
            if (!actorExists)
                ErrResponseThrower.NotFound("NOTIFICATION_ACTOR_NOT_FOUND");
        }

        if (form.NotifiersIds != null && form.NotifiersIds.Any())
        {
            foreach (var notifierId in form.NotifiersIds)
            {
                var notifierExists = await _context.Users.AnyAsync(e => e.Id == notifierId && !e.IsDeleted);
                if (!notifierExists)
                    ErrResponseThrower.NotFound("NOTIFIER_NOT_FOUND");
            }
        }

        var notifications = form.NotifiersIds.Select(e => new Notification()
        {
            ActorId = form.ActorId,
            NotifierId = e,

            Type = form.Type,

            Title = form.Title,
            Content = form.Title,
            HelperId = form.HelperId
        });
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();

    }
    
    private async Task SendNotificationThroughOneSignal(NotificationForm form)
    {
#if STOP_REAL_NOTIFICATIONS
        return;
#endif
        using HttpClient httpClient = new();

        var request = new HttpRequestMessage(HttpMethod.Post, ONE_SIGNAL_PUSH_NOTIFICATIONS_URL);
        request.Headers.Add(HeaderNames.Authorization, $"Basic {_appKey}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                app_id = _appId,
                included_segments = form.NotifiersIds,
                contents = new { en = form.Title },
                headings = new { en = form.Title },
                data = new
                {
                    helperId = form.HelperId
                }
            }),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        var response = await httpClient.SendAsync(request);
    }
    #endregion

    #region Read
    public async Task<Response<int>> CountUnseenNotifications(Guid notifierId)
    {
        var num = await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsSeen)
            .CountAsync();
        return new Response<int>(num, null, 200);
    }
    
    public async Task<Response<PagedList<NotificationDTO>>> GetAll(NotificationsFilter filter)
    {
        var dtos = await _context.Notifications
            .WhereBaseFilter(filter)
            
            .Where(e => filter.ActorId == null || e.ActorId == filter.ActorId)
            .Where(e => filter.NotifierId == null || e.NotifierId == filter.NotifierId)
            
            .Where(e => filter.Type == null || e.Type == filter.Type)
            .Where(e => filter.Tag == null || e.Tags.Contains(filter.Tag.Value))
            
            
            .Where(e => filter.IsSeen == null || e.IsSeen == filter.IsSeen)
            .Where(e => filter.IsOpened == null || e.IsOpened == filter.IsOpened)
            
            .Where(e => filter.HelperId == null || e.HelperId == filter.HelperId)
			
            .OrderByCreationDate()
            .ProjectTo<NotificationDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<NotificationDTO>>(dtos, null, 200);
    }
    #endregion
    
    #region Seen/Opened states
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
        var notifications = await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsSeen)
            .ExecuteUpdateAsync(set => set.SetProperty(e => e.IsSeen, true));
    }
    
    public async Task SetAllOpened(Guid notifierId)
    {
        var notifications = await _context.Notifications
            .Where(e => e.NotifierId == notifierId)
            .Where(e => !e.IsOpened)
            .ExecuteUpdateAsync(set => set.SetProperty(e => e.IsOpened, true));
    }
    #endregion

    public async Task EnsureNotifierOwnsNotification(Guid notifierId, Guid notificationId)
    {
        var userOwnsNotification = await _context.Notifications
            .Where(e => e.Id == notificationId)
            .Where(e => e.NotifierId == notifierId)
            .AnyAsync();
        if (!userOwnsNotification)
            ErrResponseThrower.BadRequest();
    }
}