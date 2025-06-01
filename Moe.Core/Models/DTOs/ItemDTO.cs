using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.DTOs;

public class ItemDTO : BaseDTO
{
    #region Auto
    public string Name { get; set; }
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public Guid CreatedByUserId { get; set; }
    #endregion

    #region Manual
    #endregion
}


public class ItemFormDTO : BaseFormDTO
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Details { get; set; }

    public Guid? WarehouseId { get; set; }

    [Range(0, int.MaxValue)]
    public int? Qty { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [BindNever]
    [JsonIgnore] 
    public Guid CreatedByUserId { get; set; }
}

public class ItemValidator : AbstractValidator<ItemFormDTO> 
{
    public ItemValidator()
    {
        RuleFor(x => x.Qty)
             .NotNull()
             .When(x => x.WarehouseId.HasValue)
             .WithMessage("Qty is required when WarehouseId is provided.");
    }
}

public class ItemUpdateDTO : BaseUpdateDTO
{
    public string? Name { get; set; }

    public string? Details { get; set; }

    public Guid? WarehouseId { get; set; }

    [Range(0, int.MaxValue)]
    public int? Qty { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Price { get; set; }

    [BindNever]
    [JsonIgnore] 
    public Guid CreatedByUserId { get; set; }
}

public class ItemFilter : BaseFilter
{
    public string? Name { get; set; }
    
    public string? Details { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public decimal? Price { get; set; }
    public Guid? CreatedByUserId { get; set; }

}
