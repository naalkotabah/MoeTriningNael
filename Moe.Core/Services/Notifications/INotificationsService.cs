using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;

namespace Moe.Core.Services;

public interface INotificationsService
{
    #region Send
    Task Send(NotificationForm form);
    #endregion

    #region Read
    Task<Response<PagedList<NotificationDTO>>> GetAll(NotificationsFilter filter);

    Task<Response<int>> CountUnseenNotifications(Guid notifierId);
    #endregion

    #region Write
    Task SwitchSeen(Guid id);
    Task SwitchOpened(Guid id);

    Task SetAllSeen(Guid notifierId);
    Task SetAllOpened(Guid notifierId);
    #endregion

    Task EnsureNotifierOwnsNotification(Guid notifierId, Guid notificationId);
}