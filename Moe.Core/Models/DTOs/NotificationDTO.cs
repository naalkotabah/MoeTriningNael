using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moe.Core.Models.Entities;

namespace Moe.Core.Models.DTOs;

public class NotificationDTO : BaseDTO
{
     #region Auto
     public Guid? ActorId { get; set; }
     public string? ActorName { get; set; }
     
     public Guid? NotifierId { get; set; }
     public string? NotifierName { get; set; }

     public NotificationType? Type { get; set; }
     public List<NotificationTag>? Tags { get; set; }
     public NotificationHelperIdType? HelperIdType { get; set; }

     public string? Title { get; set; }
     public string? Content { get; set; }
     public Guid? HelperId { get; set; }

     public bool? IsSeen { get; set; }
     public bool? IsOpened { get; set; }
     #endregion

     #region Manual
     #endregion
}

public class NotificationDTOSimplified
{
     public string? Title { get; set; }
     public string? Content { get; set; }
     public Guid? HelperId { get; set; }
     public NotificationHelperIdType? HelperIdType { get; set; }
}

public class NotificationForm : BaseFormDTO
{
     [JsonIgnore]
     [BindNever]
     public Guid? ActorId { get; set; }

     public NotificationType Type { get; set; }

     public List<NotificationTag> Tags { get; set; } = new();

     public NotificationHelperIdType HelperIdType { get; set; }


     [Required]
     [StringLength(256, MinimumLength = 4)]
     public string Title { get; set; }
     
     [MaxLength(512)]
     public string? Content { get; set; }

     public Guid? HelperId { get; set; }


     public List<Guid> NotifiersIds { get; set; } = new();
}

public class NotificationsFilter : BaseFilter
{
     public Guid? ActorId { get; set; }
     public Guid? NotifierId { get; set; }

     public NotificationType? Type { get; set; }
     public NotificationTag? Tag { get; set; }
     public NotificationHelperIdType? HelperIdType { get; set; }

     public Guid? HelperId { get; set; }
     
     public bool? IsSeen { get; set; }
     public bool? IsOpened { get; set; }
}