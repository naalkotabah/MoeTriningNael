using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moe.Core.Models.Entities;
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

    [MaxLength(20)] public string? Username { get; set; }

    public UserState IsBanned { get; set; } = UserState.Active;

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

    #region One-To-N
    public Guid? WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    #endregion

    #region Many-To-N
    public ICollection<Notification> NotificationsSent { get; set; } = new List<Notification>();
    public ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();

    public ICollection<Item> CreatedItems { get; set; } = new List<Item>();
    #endregion

}

public enum StaticRole
{
    SUPER_ADMIN = 0,
    ADMIN = 10,
    NORMAL = 20,
    WAREHOUSE_ADMIN = 30
}

public enum UserState
{
    Active = 0,
    Band = 1,
    
    
}
public class SetUserStateDTO
{

    [JsonIgnore]
    public string StaticRole { get; set; }


    public Guid UserId { get; set; }

    [Required]
    public UserState NewState { get; set; }
}
