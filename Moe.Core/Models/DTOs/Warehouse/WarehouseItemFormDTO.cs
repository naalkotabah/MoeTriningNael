using FluentValidation;
using Moe.Core.Data;
using Moe.Core.Models.DTOs.Items;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.DTOs.Warehouse
{
    public class WarehouseItemFormDTO
    {
        [JsonIgnore]
        public Guid WarehouseId { get; set; }
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
    }



    public class WarehouseItemFormValidator : AbstractValidator<WarehouseItemFormDTO>
    {
        private readonly MasterDbContext _context;

        public WarehouseItemFormValidator(MasterDbContext context)
        {
            _context = context;

           

            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("المنتج مطلوب.")
                .Must(ItemExists).WithMessage("المنتج المحدد غير موجود.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من 0.");
        }


     
        private bool ItemExists(Guid itemId)
        {
            return _context.items.Any(i => i.Id == itemId);
        }
    }

}
