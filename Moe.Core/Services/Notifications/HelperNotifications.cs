using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface IHelperNotifications
{
    Task Send_Dummy();
}
public class HelperNotifications : IHelperNotifications
{
    private readonly INotificationsService _notificationsService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MasterDbContext _context;

    public HelperNotifications(INotificationsService notificationsService, IHttpContextAccessor httpContextAccessor, MasterDbContext context)
    {
        _notificationsService = notificationsService;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task Send_Dummy()
    {
        var notificationForm = new NotificationForm()
        {
            ActorId = _httpContextAccessor.HttpContext.GetCurUserId(),
            Type = NotificationType.DUMMY,

            Title = $"This is a dummy notification title",
            Content = $"This is a dummy notification content",
            
            NotifiersIds = await _context.Users
                .Select(e => e.Id)
                .ToListAsync()
        };

        await _notificationsService.Send(notificationForm);
    }
}