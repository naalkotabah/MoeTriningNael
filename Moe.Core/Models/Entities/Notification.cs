using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Moe.Core.Models.Entities;

public class Notification : BaseEntity
{
    #region One-To-N
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User? Actor { get; set; }
    public Guid? ActorId { get; set; }
    
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User? Notifier { get; set; }
    public Guid? NotifierId { get; set; }
    #endregion

    #region Non-Functional
    public NotificationType Type { get; set; }
    public List<NotificationTag> Tags { get; set; }
    public NotificationHelperIdType? HelperIdType { get; set; }
    
    
    [MaxLength(256)]
    public string Title { get; set; }
    [MaxLength(512)]
    public string? Content { get; set; }
    public Guid? HelperId { get; set; }
    
    public bool IsSeen { get; set; } = false;
    public bool IsOpened { get; set; } = false;
    #endregion
}

public enum NotificationType 
{
    BY_DASHBOARD = 0,
        
    DUMMY = 69
}

public enum NotificationTag
{
    ADMINS = 0,
    ADS = 10,
    DUMMY = 69
}

public enum NotificationHelperIdType
{
    
}