using FluentValidation;
using Moe.Core.Models.DTOs.Auth;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.DTOs.Warehouse
{
    public class WarehouseFormDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public List<Guid>? AdminIds { get; set; }
    }
    public class WarehouseFormValidator : AbstractValidator<WarehouseFormDTO>
    {
        public WarehouseFormValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المستودع مطلوب")
                .MaximumLength(100).WithMessage("اسم المستودع يجب أن لا يتجاوز 100 حرف");

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("الموقع يجب أن لا يتجاوز 200 حرف")
                .When(x => !string.IsNullOrWhiteSpace(x.Location));

            RuleForEach(x => x.AdminIds)
                .NotEmpty().WithMessage("يجب أن يحتوي كل معرف مدير على قيمة صحيحة")
                .When(x => x.AdminIds != null && x.AdminIds.Any());
        }
    }





    public class WarehouseAdminFormDTO
    {
        [Required, StringLength(128, MinimumLength = 3)]
        public string Name { get; set; }

        [EmailAddress, MaxLength(320)]
        public string Email { get; set; }


      

        [MaxLength(20)]
        public string? Username { get; set; }

        [MaxLength(16)]
        public string? Phone { get; set; }

        [MaxLength(8)]
        public string? PhoneCountryCode { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }


    public class WarehouseAdminFormValidator : AbstractValidator<WarehouseAdminFormDTO>

    {
        public WarehouseAdminFormValidator()
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


    public class WarehouseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }


        public List<string> AdminNames { get; set; } = new();

        public List<WarehouseItemDTO> Items { get; set; } = new();


    }
    public class WarehouseItemDTO
    {
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
    }

}
