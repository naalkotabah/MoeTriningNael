using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.DTOs;

public class WarehouseItemTransactionDTO : BaseDTO
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }
    public string? FromWarehouseName { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string? ToWarehouseName { get; set; } 
    public Guid ItemId { get; set; }
    public string? ItemName { get; set; }          
    #endregion
    #region Functional
    [Range(0, int.MaxValue)]
    public int Qtu { get; set; }
    #endregion

    #region Manual
    #endregion
}

public class WarehouseItemTransactionFormDTO : BaseFormDTO
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }

    [Required]
    public Guid ToWarehouseId { get; set; }

    [Required]
    public Guid ItemId { get; set; }
    #endregion

    #region Functional
    [Required]
    [Range(1, int.MaxValue)]
    public int Qtu { get; set; }
    #endregion
}
public class WarehouseItemTransactionValidator : AbstractValidator<WarehouseItemTransactionFormDTO>
{
  public WarehouseItemTransactionValidator()
    {
        RuleFor(x => x.ToWarehouseId)
            .NotEmpty().WithMessage("ToWarehouseId is required.");
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");
        RuleFor(x => x.Qtu)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
public class WarehouseItemTransactionUpdateDTO : BaseUpdateDTO
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }

    [Required]
    public Guid ToWarehouseId { get; set; }

    [Required]
    public Guid ItemId { get; set; }
    #endregion

    #region Functional
    [Range(1, int.MaxValue)]
    public int Qtu { get; set; }
    #endregion
}

public class WarehouseItemTransactionFilter : BaseFilter
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public Guid? ItemId { get; set; }
    #endregion

    #region Functional
    [Range(0, int.MaxValue)]
    public int? Qtu { get; set; }
    #endregion
}
