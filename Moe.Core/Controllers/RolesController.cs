using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Models.DTOs;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Services;

namespace Moe.Core.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class RolesController : BaseController
{
	private readonly IRolesService _rolesService;
	
	public RolesController(IRolesService rolesService)
	{
		_rolesService = rolesService;
	}

	#region CRUDs
	[TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
	[HttpGet]
	public async Task<ActionResult<Response<PagedList<RoleDTO>>>> GetAll([FromQuery] RoleFilter filter) =>
		Ok(await _rolesService.GetAll(filter));

	[HttpGet("{id}")]
	public async Task<ActionResult<Response<RoleDTO>>> GetById(Guid id) =>
		Ok(await _rolesService.GetById(id));

	[HttpPost]
	public async Task<ActionResult<Response<RoleDTO>>> Create([FromBody] RoleFormDTO form) =>
		Ok(await _rolesService.Create(form));

	[HttpPut("{id}")]
	public async Task<ActionResult<Response<RoleDTO>>> Update([FromBody] RoleUpdateDTO update, Guid id)
	{
		update.Id = id;
		await _rolesService.Update(update);
			
		return Ok();
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(204)]
	public async Task<IActionResult> Delete(Guid id)
	{
	    await _rolesService.Delete(id);
	    return Ok();
	}
	#endregion
}
