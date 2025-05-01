using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Services;

namespace Moe.Core.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class CountriesController : BaseController
{
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    #region CRUDs
    [TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
    [HttpGet]
    public async Task<ActionResult<Response<PagedList<CountryDTO>>>> GetAll([FromQuery] CountryFilter filter) =>
        Ok(await _countriesService.GetAll(filter));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Response<CountryDTO>>> GetById(Guid id) =>
        Ok(await _countriesService.GetById(id));

    [Authorize(Roles = "super-admin")]
    [HttpPost]
    public async Task<ActionResult<Response<CountryDTO>>> Create([FromBody] CountryFormDTO form) =>
        Ok(await _countriesService.Create(form));

    [Authorize(Roles = "super-admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Response<CountryDTO>>> Update(Guid id, [FromBody] CountryUpdateDTO update)
    {
        update.Id = id;
        await _countriesService.Update(update);
            
        return Ok();
    }

    [Authorize(Roles = "super-admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _countriesService.Delete(id);
        return Ok();
    }
    #endregion
}