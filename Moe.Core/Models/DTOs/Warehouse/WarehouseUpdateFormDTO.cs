using FluentValidation;
using Moe.Core.Models.DTOs.Auth;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Moe.Core.Models.DTOs.Warehouse
{
    public class WarehouseUpdateFormDTO:BaseUpdateDTO
    {

   

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        
    }



    public class WarehouseUpdateFormValidator : AbstractValidator<WarehouseUpdateFormDTO>
    {
        public WarehouseUpdateFormValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.Name) ||
        
                    !string.IsNullOrWhiteSpace(x.Location))
                .WithMessage("You must provide either Name, or Location.");

           
        }
    }

}
