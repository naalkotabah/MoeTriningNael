using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities
{
    public class InventoryMovement : BaseEntity
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public Guid FromWarehouseId { get; set; }

        [Required]
        public Guid ToWarehouseId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Quantity { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.UtcNow;


        public Item Item { get; set; }
        public Warehouse FromWarehouse { get; set; }
        public Warehouse ToWarehouse { get; set; }
    }

}
