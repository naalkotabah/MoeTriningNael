using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Moe.Core.Models.DTOs.Auth;

public class ResetPasswordFormDTO
{
    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string NewPassword { get; set; }
}



public class ForgetPasswordFormDTO
{
    public string? Email { get; set; }  
    public string? Phone { get; set; } 
    public string? PhoneCountryCode { get; set; }  
}


public class ForgetPasswordVerifyOtpFormDTO
{
 

    [Required]
    public string OTP { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string NewPassword { get; set; }
}
