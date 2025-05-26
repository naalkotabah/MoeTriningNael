using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Models.DTOs;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Services;

namespace Moe.Core.Controllers;

/// <summary>
/// Basic cruds
/// </summary>
public class WarehousesController : BaseController
{
	private readonly IWarehousesService _warehousesService;

	public WarehousesController(IWarehousesService warehousesService)
	{
		_warehousesService = warehousesService;
	}
	
	#region CRUDs
    /// <summary>
    /// Retrieves a paged list
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
	[Authorize]
	[TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
	[HttpGet]
	public async Task<ActionResult<Response<PagedList<WarehouseDTO>>>> GetAll([FromQuery] WarehouseFilter filter) =>
		Ok(await _warehousesService.GetAll(filter));

    /// <summary>
    /// Retrieves a single instance by its ID
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
	[Authorize]
	[HttpGet("{id}")]
	public async Task<ActionResult<Response<WarehouseDTO>>> GetById(Guid id) =>
		Ok(await _warehousesService.GetById(id));

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
	[Authorize]
	[HttpPost]
	public async Task<ActionResult<Response<WarehouseDTO>>> Create([Required] [FromBody] WarehouseFormDTO form) =>
		Ok(await _warehousesService.Create(form));

    /// <summary>
    /// Updates an instance determined by its ID
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
	[Authorize]
	[HttpPut("{id}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Update([Required] [FromBody] WarehouseUpdateDTO update, Guid id)
	{
		update.Id = id;
		await _warehousesService.Update(update);
		
		return Ok();
	}

    /// <summary>
    /// Deletes an instance determined by its ID
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
	[Authorize]
	[HttpDelete("{id}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Delete(Guid id)
	{
	    await _warehousesService.Delete(id);
	    return Ok();
	}
	#endregion
}
