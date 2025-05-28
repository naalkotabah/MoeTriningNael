using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;

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
	public ItemsService(MasterDbContext context, IMapper mapper): base(context, mapper)
	{}

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
	    var dto = await _context.CreateWithMapper<Item,ItemDTO>(form, _mapper);

		return new Response<ItemDTO>(dto, null, 200);
	}

	public async Task Update(ItemUpdateDTO update)
	{
	    await _context.UpdateWithMapperOrException<Item,ItemUpdateDTO>(update, _mapper);
	}

	public async Task Delete(Guid id) =>
	    await _context.SoftDeleteOrException<Item>(id);
}
