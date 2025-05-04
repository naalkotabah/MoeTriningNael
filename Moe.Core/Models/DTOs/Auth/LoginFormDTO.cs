using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Moe.Core.Models.DTOs.Auth;

public class LoginFormDTO 
{
    public string? Email { get; set; }
    public string? PhoneCountryCode { get; set; }
    public string? Phone { get; set; }
    public string? Username { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string Password { get; set; }


}


public class LoginFormFluentValidator : AbstractValidator<LoginFormDTO>
{
    public LoginFormFluentValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.Email) ||
                (!string.IsNullOrWhiteSpace(x.Phone) && !string.IsNullOrWhiteSpace(x.PhoneCountryCode)) ||
                !string.IsNullOrWhiteSpace(x.Username))
            .WithMessage("You must provide either Email, Phone with CountryCode, or Username.");

        RuleFor(x => x.PhoneCountryCode)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
