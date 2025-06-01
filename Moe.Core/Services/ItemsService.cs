using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Null;
using Moe.Core.Migrations;

namespace Moe.Core.Services;

public interface IItemsService
{
    Task<Response<PagedList<ItemDTO>>> GetAll(ItemFilter filter);
    Task<Response<ItemDTO>> GetById(Guid id);
    Task<Response<ItemDTO>> Create(ItemFormDTO form);
    Task Update(ItemUpdateDTO update);
    Task Delete(Guid id);
}

public class ItemsService : BaseService, IItemsService
{
    public ItemsService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    { }

    public async Task<Response<PagedList<ItemDTO>>> GetAll(ItemFilter filter)
    {
        var items = await _context.Items
            .WhereBaseFilter(filter)
            .OrderByCreationDate()
            .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<ItemDTO>>(items, null, 200);
    }

    public async Task<Response<ItemDTO>> GetById(Guid id)
    {
        var dto = await _context.GetByIdOrException<Item, ItemDTO>(id);
        return new Response<ItemDTO>(dto, null, 200);
    }
    public async Task<Response<ItemDTO>> Create(ItemFormDTO form)
    {
      
        var dto = await _context.CreateWithMapper<Item, ItemDTO>(form, _mapper);

        if (form.WarehouseId.HasValue)
        {
            await _context.EnsureEntityExists<Warehouse>(form.WarehouseId);
            var warehouseItem = new WarehouseItem
            {
                WarehouseId = form.WarehouseId.GetValueOrDefault(),
                ItemId = dto.Id.GetValueOrDefault(),
                Qty = form.Qty.GetValueOrDefault(0)
            };

            await _context.AddAsync(warehouseItem);
            await _context.SaveChangesAsync();
        }

        return new Response<ItemDTO>(dto, null, 200);
    }

    public async Task Update(ItemUpdateDTO update)
    {
        
        await _context.UpdateWithMapperOrException<Item, ItemUpdateDTO>(update, _mapper);

        if (update.WarehouseId.HasValue)
        {
            var warehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(wi => wi.ItemId == update.Id);

            if (warehouseItem != null)
            {
                warehouseItem.WarehouseId = update.WarehouseId.GetValueOrDefault();
                warehouseItem.Qty = update.Qty.GetValueOrDefault();
            }
            else
            {
                var newWarehouseItem = new WarehouseItem
                {
                    WarehouseId = update.WarehouseId.GetValueOrDefault(),
                    ItemId = update.Id,
                    Qty = update.Qty.GetValueOrDefault()
                };
                await _context.AddAsync(newWarehouseItem);
            }
        }

        await _context.SaveChangesAsync();
    }



    public async Task Delete(Guid id)=>
      await _context.SoftDeleteOrException<Item>(id);
    



}
