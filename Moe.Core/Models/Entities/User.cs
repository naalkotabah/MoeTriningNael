using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.Entities;

public class User : BaseEntity
{
    #region Auth
    public StaticRole StaticRole { get; set; }
    
    [MaxLength(320)] public string? Email { get; set; }
    
    [MaxLength(16)] public string? Phone { get; set; }
    [MaxLength(8)] public string? PhoneCountryCode { get; set; }
    
    [JsonIgnore] public byte[] PasswordHash { get; set; }
    [JsonIgnore] public byte[] PasswordSalt { get; set; }
    #endregion

    #region Functional

    public Lang Lang { get; set; }
    #endregion

    #region Non-Functional
    [MaxLength(128)] public string Name { get; set; }
    
    [MaxLength(64)] public string? ProfileImg { get; set; }
    [MaxLength(64)] public string? CoverImg { get; set; }
    #endregion

    #region Many-To-N
    public ICollection<Notification> NotificationsSent { get; set; } = new List<Notification>();
    public ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();
    #endregion
}

public enum StaticRole
{
    SUPER_ADMIN = 0,
    ADMIN = 10,
    NORMAL = 20
}