using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities;

public class WarehouseItemTransaction : BaseEntity
{
    #region One-To-N
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Warehouse From { get; set; }
    public Guid? FromId { get; set; }


    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Warehouse To { get; set; }
    public Guid ToId { get; set; }
    

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Item Item { get; set; }
    public Guid ItemId { get; set; }
 
    #endregion
    #region Functional
    [Range(0, int.MaxValue)]
    public int Qty { get; set; }
    #endregion
}

