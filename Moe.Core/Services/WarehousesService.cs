using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface IWarehousesService
{
	Task<Response<PagedList<WarehouseDTO>>> GetAll(WarehouseFilter filter);
	Task<Response<WarehouseDTO>> GetById(Guid id);
	Task<Response<WarehouseDTO>> Create(WarehouseFormDTO form);
	Task Update(WarehouseUpdateDTO update);
	Task Delete(Guid id);
}

public class WarehousesService : BaseService, IWarehousesService
{
	public WarehousesService(MasterDbContext context, IMapper mapper): base(context, mapper)
	{}

	public async Task<Response<PagedList<WarehouseDTO>>> GetAll(WarehouseFilter filter)
	{
		var warehouses = await _context.Warehouses
			.WhereBaseFilter(filter)
			.OrderByCreationDate()
			.ProjectTo<WarehouseDTO>(_mapper.ConfigurationProvider)
			.Paginate(filter);

		return new Response<PagedList<WarehouseDTO>>(warehouses, null, 200);
	}

	public async Task<Response<WarehouseDTO>> GetById(Guid id)
	{
        var dto = await _context.GetByIdOrException<Warehouse, WarehouseDTO>(id);
		return new Response<WarehouseDTO>(dto, null, 200);
	}

	public async Task<Response<WarehouseDTO>> Create(WarehouseFormDTO form)
	{
	    var dto = await _context.CreateWithMapper<Warehouse,WarehouseDTO>(form, _mapper);

		return new Response<WarehouseDTO>(dto, null, 200);
	}

	public async Task Update(WarehouseUpdateDTO update)
	{
	    await _context.UpdateWithMapperOrException<Warehouse,WarehouseUpdateDTO>(update, _mapper);
	}

	public async Task Delete(Guid id) =>
	    await _context.SoftDeleteOrException<Warehouse>(id);
}
