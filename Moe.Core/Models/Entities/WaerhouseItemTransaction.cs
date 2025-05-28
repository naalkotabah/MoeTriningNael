using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities;

public class WarehouseItemTransaction : BaseEntity
{
    #region One-To-N
    public Guid? FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; }
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    #endregion
    #region Functional
    [Range(0, int.MaxValue)]
    public int Qtu { get; set; }
    #endregion
}

