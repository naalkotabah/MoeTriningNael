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
        if (form.WarehouseId.HasValue)
        {
            await _context.EnsureEntityExists<Warehouse>(form.WarehouseId);
        }
        var dto = await _context.CreateWithMapper<Item, ItemDTO>(form, _mapper);

        if (form.WarehouseId.HasValue && dto?.Id != null)
        {
            var warehouseItem = new WarehouseItem
            {
                WarehouseId = form.WarehouseId.Value,
                ItemId = dto.Id.Value,
                Qty = form.Qty ?? 0
            };

            await _context.AddAsync(warehouseItem);
            await _context.SaveChangesAsync();
        }
        return new Response<ItemDTO>(dto, null, 200);
    }
    public async Task Update(ItemUpdateDTO update)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == update.Id);

        if (item == null)
            ErrResponseThrower.NotFound("item not found.");

        if (update.WarehouseId.HasValue)
        {
            var warehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(wi => wi.ItemId == update.Id);

            if (warehouseItem != null)
            {

                warehouseItem.WarehouseId = update.WarehouseId.Value;
                warehouseItem.Qty = update.Qty ?? 0;
            }
            else
            {
                var newWarehouseItem = new WarehouseItem
                {
                    WarehouseId = update.WarehouseId.Value,
                    ItemId = update.Id,
                    Qty = update.Qty ?? item.Qty ?? 0
                };
                await _context.AddAsync(newWarehouseItem);
            }

            await _context.SaveChangesAsync();
        }

        await _context.UpdateWithMapperOrException<Item, ItemUpdateDTO>(update, _mapper);
    }


    public async Task Delete(Guid id)
    {
        await _context.SoftDeleteOrException<Item>(id);

        var relatedWarehouseItems = await _context.WarehouseItems
            .Where(w => w.ItemId == id && !w.IsDeleted)
            .ToListAsync();

        foreach (var item in relatedWarehouseItems)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }



}
