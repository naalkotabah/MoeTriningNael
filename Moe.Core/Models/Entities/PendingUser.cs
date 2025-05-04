using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Moe.Core.Models.Entities;

public class PendingUser : BaseEntity
{
    #region Functional
    public string OTP { get; set; }
    public int LeftTrials { get; set; } = 6;
    #endregion

    #region Non-Functional
    [MaxLength(128)] public string? Email { get; set; }

    [MaxLength(16)] public string? Phone { get; set; }
    [MaxLength(128)] public string Name { get; set; }

    [Required]
    [MaxLength(20)]
    public string Username { get; set; } 
    [MaxLength(8)] public string? PhoneCountryCode { get; set; }

    [JsonIgnore] public Byte[] PasswordHash { get; set; }
    [JsonIgnore] public Byte[] PasswordSalt { get; set; }
    #endregion
}