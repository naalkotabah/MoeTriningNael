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


public class WarehouseItemTransactionFilter : BaseFilter
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public Guid? ItemId { get; set; }
    #endregion

    #region Functional
    [Range(0, int.MaxValue)]
    public int? MinQty { get; set; } 

    [Range(0, int.MaxValue)]
    public int? MaxQty{ get; set; }
    #endregion
}
