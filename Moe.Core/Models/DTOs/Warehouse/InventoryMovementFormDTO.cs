using FluentValidation;
using Moe.Core.Data;

namespace Moe.Core.Models.DTOs.Warehouse
{

    public class InventoryMovementFormDTO
    {
        public Guid ItemId { get; set; }           
        public Guid FromWarehouseId { get; set; }     
        public Guid ToWarehouseId { get; set; }       
        public decimal Quantity { get; set; }          
    }


    public class InventoryMovementFormValidator : AbstractValidator<InventoryMovementFormDTO>
    {
        private readonly MasterDbContext _context;

        public InventoryMovementFormValidator(MasterDbContext context)
        {
            _context = context;

           
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("المنتج مطلوب.")
                .Must(ItemExists).WithMessage("المنتج المحدد غير موجود.");

          
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من 0.");

          
            RuleFor(x => x.FromWarehouseId)
                .NotEmpty().WithMessage("المخزن المصدر مطلوب.")
                .Must(WarehouseExists).WithMessage("المخزن المصدر غير موجود.");

         
            RuleFor(x => x.ToWarehouseId)
                .NotEmpty().WithMessage("المخزن الهدف مطلوب.")
                .Must(WarehouseExists).WithMessage("المخزن الهدف غير موجود.");
        }

        private bool ItemExists(Guid itemId)
        {
            return _context.items.Any(i => i.Id == itemId);
        }

        private bool WarehouseExists(Guid warehouseId)
        {
            return _context.warehouses.Any(w => w.Id == warehouseId);
        }
    }


}
