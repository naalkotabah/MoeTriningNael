using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface IRolesService
{
	Task<Response<PagedList<RoleDTO>>> GetAll(RoleFilter filter);
	Task<Response<RoleDTO>> GetById(Guid id);
	Task<Response<RoleDTO>> Create(RoleFormDTO form);
	Task Update(RoleUpdateDTO update);
	Task Delete(Guid id);
}

public class RolesService : BaseService, IRolesService
{
	public RolesService(MasterDbContext context, IMapper mapper): base(context, mapper)
	{}

	public async Task<Response<PagedList<RoleDTO>>> GetAll(RoleFilter filter)
	{
		var roles = await _context.Roles
			.WhereBaseFilter(filter)
			
			.Where(e => filter.Name == null || e.Name.ToLower().Contains(filter.Name.ToLower()))
			
			.OrderByCreationDate()
			.ProjectTo<RoleDTO>(_mapper.ConfigurationProvider)
			.Paginate(filter);

		return new Response<PagedList<RoleDTO>>(roles, null, 200);
	}

	public async Task<Response<RoleDTO>> GetById(Guid id)
	{
		var dto = await _context.GetByIdOrException<Role, RoleDTO>(id);
		return new Response<RoleDTO>(dto, null, 200);
	}

	public async Task<Response<RoleDTO>> Create(RoleFormDTO form)
	{
		var dto = await _context.CreateWithMapper<Role, RoleDTO>(form, _mapper);
		return new Response<RoleDTO>(dto, null, 200);
	}

	public async Task Update(RoleUpdateDTO update) =>
		await _context.UpdateWithMapperOrException<Role, RoleUpdateDTO>(update, _mapper);

	public async Task Delete(Guid id) =>
		await _context.SoftDeleteOrException<Role>(id);
}
