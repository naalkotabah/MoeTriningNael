using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services
{
    public interface IWarehouseItemService
    {
        Task<Response<string>> AddItemToWarehouse(WarehouseItemFormDTO form);
        Task<Response<string>> RemoveItemFromWarehouse(Guid warehouseId, Guid itemId);
    }

    public class WarehouseItemService : BaseService, IWarehouseItemService
    {

        public WarehouseItemService(MasterDbContext context, IMapper mapper) : base(context, mapper) { }


        public async Task<Response<string>> AddItemToWarehouse(WarehouseItemFormDTO form)
        {
            if (!await _context.warehouses.AnyAsync(w => w.Id == form.WarehouseId))
                return new Response<string>(null, "المخزن المحدد غير موجود.", 404);

            var warehouseItem = await _context.WarehouseItems
           .FirstOrDefaultAsync(wi => wi.WarehouseId == form.WarehouseId && wi.ItemId == form.ItemId);

            string message;

            if (warehouseItem != null)
            {
                warehouseItem.Quantity += form.Quantity;
                _context.WarehouseItems.Update(warehouseItem);
                message = "تم تحديث كمية المنتج في المخزن بنجاح";
            }
            else
            {
                warehouseItem = _mapper.Map<WarehouseItem>(form);
                await _context.WarehouseItems.AddAsync(warehouseItem);
                message = "تم إضافة المنتج إلى المخزن بنجاح";
            }

            await _context.SaveChangesAsync();
            return new Response<string>(message, null, 200);

        }
        public async Task<Response<string>> RemoveItemFromWarehouse(Guid warehouseId, Guid itemId)
        {
            var warehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.ItemId == itemId);

            if (warehouseItem == null)
                return new Response<string>(null, "المنتج غير موجود في هذا المخزن", 404);

            _context.WarehouseItems.Remove(warehouseItem);
            await _context.SaveChangesAsync();

            return new Response<string>("تم حذف المنتج من المخزن بنجاح", null, 200);
        }


    }
}
