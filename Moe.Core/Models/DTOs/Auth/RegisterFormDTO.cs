using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Moe.Core.Models.DTOs.Auth;

public class RegisterFormDTO
{
    [Required] [EmailAddress] [MaxLength(128)] public string Email { get; set; }

    [Required] 
    [MaxLength(10)] 
    public string PhoneCountryCode { get; set; }
    [Required] [Phone] [MaxLength(20)] public string Phone { get; set; }

    public string? Name { get; set; }


    [Required]
    [MaxLength(20)]
    public string? Username { get; set; } 

    [Required] public OtpDestination OtpDestination { get; set; }
    
    [StringLength(30, MinimumLength = 6)]
    public string Password { get; set; }
}


public class RegisterFormValidator : AbstractValidator<RegisterFormDTO>
{
    public RegisterFormValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .NotNull()
            .When(e => e.Phone == null);

        RuleFor(e => e.PhoneCountryCode)
            .NotEmpty()
            .NotNull()
            .When(e => !string.IsNullOrEmpty(e.Phone));


       
    }
}


public enum OtpDestination
{
    EMAIL,
    PHONE
}