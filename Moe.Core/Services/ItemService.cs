using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Items;
using Moe.Core.Models.Entities;
using Moe.Core.Services;
using static ItemService;

public class ItemService : BaseService, IItemService
{

    public interface IItemService
    {
        Task<Response<ItemDTO>> Create(ItemFormDTO form);
        Task<Response<PagedList<ItemDTO>>> GetAll(ItemFilterDTO filter);
        Task<Response<ItemDTO>> GetById(Guid id);
        Task<Response<ItemDTO>> Update(Guid id, ItemFormDTO form);
        Task<Response<string>> Delete(Guid id);
    }
    public ItemService(MasterDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<Response<ItemDTO>> Create(ItemFormDTO form)
    {
        var item = _mapper.Map<Item>(form);
        _context.items.Add(item);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<ItemDTO>(item);
        return new Response<ItemDTO>(dto, "تم إنشاء المنتج", 201);
    }

    public async Task<Response<PagedList<ItemDTO>>> GetAll(ItemFilterDTO filter)
    {
        var query = _context.items.AsQueryable();

        if (filter.Id != null)
            query = query.Where(i => i.Id == filter.Id);

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(i => i.Name.Contains(filter.Name));

        if (!string.IsNullOrEmpty(filter.Code))
            query = query.Where(i => i.Code.Contains(filter.Code));

        if (filter.IsDeleted != null)
            query = query.Where(i => i.IsDeleted == filter.IsDeleted);

        if (filter.StartDate != null)
            query = query.Where(i => i.CreatedAt >= filter.StartDate);

        if (filter.EndDate != null)
            query = query.Where(i => i.CreatedAt <= filter.EndDate);

        var items = await query
            .OrderBy(i => i.Name)
            .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<ItemDTO>>(items, null, 200);
    }


    public async Task<Response<ItemDTO>> GetById(Guid id)
    {
        var item = await _context.items.FindAsync(id);
        if (item == null)
            return new Response<ItemDTO>(null, "المنتج غير موجود", 404);

        var dto = _mapper.Map<ItemDTO>(item);
        return new Response<ItemDTO>(dto, null, 200);
    }

    public async Task<Response<ItemDTO>> Update(Guid id, ItemFormDTO form)
    {
        var item = await _context.items.FindAsync(id);
        if (item == null)
            return new Response<ItemDTO>(null, "المنتج غير موجود", 404);

        _mapper.Map(form, item);
        _context.items.Update(item);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<ItemDTO>(item);
        return new Response<ItemDTO>(dto, "تم تحديث المنتج", 200);
    }

    public async Task<Response<string>> Delete(Guid id)
    {
        var item = await _context.items.FindAsync(id);
        if (item == null)
            return new Response<string>(null, "المنتج غير موجود", 404);

        _context.items.Remove(item);
        await _context.SaveChangesAsync();

        return new Response<string>("تم حذف المنتج", null, 200);
    }
}
