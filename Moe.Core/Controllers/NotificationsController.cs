using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Services;

namespace Moe.Core.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class NotificationsController : BaseController
{
    private readonly INotificationsService _notificationsService;
    private readonly IHelperNotifications _helperNotifications;

    public NotificationsController(INotificationsService notificationsService, IHelperNotifications helperNotifications)
    {
        _notificationsService = notificationsService;
        _helperNotifications = helperNotifications;
    }

    #region Read
    /// <summary>
    /// Retrieves a paged list of your notifications
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    ///
    /// Notes:
    /// - `notifierId` is enforced is equal your user Id.
    ///
    /// Guide
    /// ---
    /// DUMMY = 69:
    /// - Sent to: all users.
    /// - helperId: none.
    /// </remarks>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<Response<PagedList<NotificationDTO>>>> GetAll([FromQuery] NotificationsFilter filter)
    {
        filter.NotifierId = CurId;
        return Ok(await _notificationsService.GetAll(filter));
    }
    
    /// <summary>
    /// Returns the counr of your unseen notifications
    /// </summary>
    /// <remarks>
    /// Required Roles: `None`
    /// </remarks>
    [HttpGet("count-unseen")]
    public async Task<ActionResult<Response<string>>> CountUnseen() =>
        Ok(await _notificationsService.CountUnseenNotifications(CurId));
    #endregion

    #region Write
    /// <summary>
    /// Switch the seen boolean of your notification
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    ///
    /// Notes:
    /// - If you tried to modify a notification that isn't yours you'd get a 400 even if you're an admin trying to modify external users notifications.
    /// </remarks>
    [Authorize]
    [HttpPut("{id}/switch-seen")]
    public async Task<IActionResult> SwitchSeen(Guid id)
    {
        await _notificationsService.EnsureNotifierOwnsNotification(CurId, id);
        await _notificationsService.SwitchSeen(id);
        return Ok();
    }

    /// <summary>
    /// Sets all your notifications as seen
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
    [Authorize]
    [HttpPut("set-all-seen")]
    public async Task<IActionResult> SetAllSeen()
    {
        await _notificationsService.SetAllSeen(CurId);
        return Ok();
    }

    /// <summary>
    /// Switch the opened boolean of your notification
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    ///
    /// Notes:
    /// - If you tried to modify a notification that isn't yours you'd get a 400 even if you're an admin trying to modify external users notifications.
    /// </remarks>
    [Authorize]
    [HttpPut("{id}/switch-opened")]
    public async Task<IActionResult> SwitchOpened(Guid id)
    {
        await _notificationsService.EnsureNotifierOwnsNotification(CurId, id);
        await _notificationsService.SwitchOpened(id);
        return Ok();
    }
    
    /// <summary>
    /// Sets all your notifications as seen
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
    [Authorize]
    [HttpPut("set-all-opened")]
    public async Task<IActionResult> SetAllOpened()
    {
        await _notificationsService.SetAllOpened(CurId);
        return Ok();
    }
    #endregion

    #region Dummy
    /// <summary>
    /// Sends a dummy notification to all users for testing
    /// </summary>
    [Authorize]
    [HttpPost("dummy")]
    public async Task<IActionResult> Dummy()
    {
        await _helperNotifications.Send_Dummy();
        return Ok();
    }
    #endregion
}