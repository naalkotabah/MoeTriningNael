using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities
{
    public class Warehouse : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }





        public ICollection<User> Admins { get; set; } = new List<User>();
        public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
    }

}
