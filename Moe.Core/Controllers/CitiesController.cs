using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Services;

namespace Moe.Core.Controllers;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class CitiesController : BaseController
{
    private readonly ICitiesService _citiesService;

    public CitiesController(ICitiesService citiesService)
    {
        _citiesService = citiesService;
    }

    #region CRUDs
    [TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Response<PagedList<CityDTO>>>> GetAll([FromQuery] CityFilter filter) =>
        Ok(await _citiesService.GetAll(filter));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Response<CityDTO>>> GetById(Guid id) =>
        Ok(await _citiesService.GetById(id));

    [Authorize(Roles = "super-admin")]
    [HttpPost]
    public async Task<ActionResult<Response<CityDTO>>> Create([FromBody] CityFormDTO form) =>
        Ok(await _citiesService.Create(form));

    [Authorize(Roles = "super-admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<CityDTO>>> Update(Guid id, [FromBody] CityUpdateDTO update)
    {
        update.Id = id;
        await _citiesService.Update(update);
            
        return Ok();
    }

    [Authorize(Roles = "super-admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _citiesService.Delete(id);
        return Ok();
    }
    #endregion
}