using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities
{
    public class Item : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

       

        [MaxLength(50)]
        public string? Unit { get; set; }

        public string? ImagePath { get; set; }

        public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
    }

}
