using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Moe.Core.Services
{
    public interface IInventoryMovementService
    {
        Task<Response<string>> TransferProductBetweenWarehouses(InventoryMovementFormDTO form);
    }

    public class InventoryMovementService : BaseService, IInventoryMovementService
    {
        public InventoryMovementService(MasterDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<Response<string>> TransferProductBetweenWarehouses(InventoryMovementFormDTO form)
        {
            var fromWarehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(wi => wi.WarehouseId == form.FromWarehouseId && wi.ItemId == form.ItemId);

            if (fromWarehouseItem == null || fromWarehouseItem.Quantity < form.Quantity)
                return new Response<string>(null, "الكمية غير متوفرة في المخزن المصدر", 400);
       
            var toWarehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(wi => wi.WarehouseId == form.ToWarehouseId && wi.ItemId == form.ItemId);

            if (toWarehouseItem == null)
            {
 
                toWarehouseItem = _mapper.Map<WarehouseItem>(form);
                _context.WarehouseItems.Add(toWarehouseItem);
            }
            else
            {
    
                toWarehouseItem.Quantity += form.Quantity;
                _context.WarehouseItems.Update(toWarehouseItem);
            }

 
            fromWarehouseItem.Quantity -= form.Quantity;
            _context.WarehouseItems.Update(fromWarehouseItem);

   
            var movement = _mapper.Map<InventoryMovement>(form);
            movement.MovementDate = DateTime.UtcNow;

            _context.InventoryMovements.Add(movement);

          
            await _context.SaveChangesAsync();

            return new Response<string>("تم تحويل المنتج بين المخازن بنجاح", null, 200);
        }

    }
} 
