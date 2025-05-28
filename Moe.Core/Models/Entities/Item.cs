namespace Moe.Core.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item : BaseEntity
{
    #region One-To-N
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(500)]
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }
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
