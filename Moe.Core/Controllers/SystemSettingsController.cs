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
/// Overall system settings suchs as urls or restrictions
/// </summary>
/// 
[ApiExplorerSettings(IgnoreApi = true)]
public class SystemSettingsController : BaseController
{
	private readonly ISystemSettingsService _systemSettingsService;

	public SystemSettingsController(ISystemSettingsService systemSettingsService)
	{
		_systemSettingsService = systemSettingsService;
	}
	
	#region CRUDs

	/// <summary>
	/// Retrieves the system settings
	/// </summary>
	/// <remarks>
	/// Required Roles: `None`
	/// </remarks>
	[HttpGet]
	public async Task<ActionResult<Response<SystemSettingsDTO>>> Get() =>
		Ok(await _systemSettingsService.Get());

	/// <summary>
	/// Updates the system settings
	/// </summary>
	/// <remarks>
	/// Required Roles: `Super Admin` `Admin`
	/// </remarks>
	[Authorize(Roles = "super-admin,admin")]
	[HttpPut]
	[ProducesResponseType(204)]
	public async Task<ActionResult<Response<SystemSettingsDTO>>> Update([FromBody] SystemSettingsUpdateDTO update)
	{
		await _systemSettingsService.Update(update);
		
		return Ok();
	}
	#endregion
}