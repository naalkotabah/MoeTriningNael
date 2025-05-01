#define STOP_REAL_NOTIFICATIONS

using FirebaseAdmin.Messaging;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Notification = FirebaseAdmin.Messaging.Notification;

namespace Moe.Core.Services;

public class FcmNotificationsService : INotificationsService
{
    private readonly MasterDbContext _context;

    public FcmNotificationsService(MasterDbContext context)
    {
        _context = context;
    }

    public Task<Response<PagedList<NotificationDTO>>> GetAll(NotificationsFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<Response<int>> CountUnseenNotifications(Guid notifierId)
    {
        throw new NotImplementedException();
    }

    public Task SwitchSeen(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SwitchOpened(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SetAllSeen(Guid notifierId)
    {
        throw new NotImplementedException();
    }

    public Task SetAllOpened(Guid notifierId)
    {
        throw new NotImplementedException();
    }

    public Task EnsureNotifierOwnsNotification(Guid notifierId, Guid notificationId)
    {
        throw new NotImplementedException();
    }

    public async Task Send(NotificationForm form)
    {
        #if false
        if (!form.ActorId.IsNullOrZeros())
            await _context.EnsureEntityExists<User>(form.ActorId.Value, "Actor", "المرسل");
        await _context.EnsureEntitiesIdsExists<User>(form.NotifiersIds, "Notifier", "المرسل له");
        
        var tasks = form.Channels
            .Select(channel => FirebaseMessaging.DefaultInstance.SendAsync(new Message()
            {
                Topic = channel,
                Notification = new Notification()
                {
                    Title = "TODO title",
                    Body = "TODO body"
                }
            }))
            .ToList();

        await Task.WhenAll(tasks);
        #endif
    }
}