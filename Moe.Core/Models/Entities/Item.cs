namespace Moe.Core.Models.Entities;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item : BaseEntity
{
    #region One-To-N
    [MaxLength(100)]
    public string Name { get; set; } 

    [MaxLength(500)]
    public string Details { get; set; }

    public Guid? WarehouseId { get; set; }  
    public int? Qty { get; set; }

    public string? ImageUrl { get; set; } 

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User CreatedByUser { get; set; }
    public Guid CreatedByUserId { get; set; }
    #endregion

    #region Functional
    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }
    #endregion
    #region Non-Functional
    #endregion

    #region Many-To-N
    public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
    #endregion
}
