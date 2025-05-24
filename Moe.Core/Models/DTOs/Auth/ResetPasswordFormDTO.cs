using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Moe.Core.Models.DTOs.Auth;

public class ResetPasswordFormDTO
{
    [JsonIgnore]
    [BindNever]
    public Guid UserId { get; set; }

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

public class ForgetPasswordFormFluentValidator : AbstractValidator<ForgetPasswordFormDTO>
{
    public ForgetPasswordFormFluentValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                (!string.IsNullOrWhiteSpace(x.Email) && string.IsNullOrWhiteSpace(x.Phone) && string.IsNullOrWhiteSpace(x.PhoneCountryCode)) ||
                (string.IsNullOrWhiteSpace(x.Email) && !string.IsNullOrWhiteSpace(x.Phone) && !string.IsNullOrWhiteSpace(x.PhoneCountryCode))
            )
            .WithMessage("You must provide either Email, or Phone with CountryCode.");

        RuleFor(x => x.PhoneCountryCode)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("PhoneCountryCode is required when Phone is provided.");
    }
}


public class ForgetPasswordVerifyOtpFormDTO
{
 

    [Required]
    public string OTP { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string NewPassword { get; set; }
}
