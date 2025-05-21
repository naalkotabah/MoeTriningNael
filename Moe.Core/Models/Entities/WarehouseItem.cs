using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities
{
    public class WarehouseItem : BaseEntity
    {
        [Required]
        public Guid WarehouseId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }





        public Warehouse Warehouse { get; set; }
        public Item Item { get; set; }
    }

}
