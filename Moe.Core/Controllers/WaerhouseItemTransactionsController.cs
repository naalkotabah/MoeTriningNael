using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Models.DTOs;
using Moe.Core.ActionFilters;
using Moe.Core.Services;
using Moe.Core.Helpers;

namespace Moe.Core.Controllers;

/// <summary>
/// Basic CRUDs
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WarehouseItemTransactionsController : BaseController
{
    private readonly IWarehouseItemTransactionsService _warehouseItemTransactionsService;

    public WarehouseItemTransactionsController(IWarehouseItemTransactionsService warehouseItemTransactionsService)
    {
        _warehouseItemTransactionsService = warehouseItemTransactionsService;
    }

    #region CRUDs

    /// <summary>
    /// Retrieves a paged list of warehouse item transactions.
    /// </summary>
    /// <remarks>Required Roles: Any</remarks>
    [Authorize]
    [TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
    [HttpGet]
    public async Task<ActionResult<Response<PagedList<WarehouseItemTransactionDTO>>>> GetAll([FromQuery] WarehouseItemTransactionFilter filter)
        => Ok(await _warehouseItemTransactionsService.GetAll(filter));

    /// <summary>
    /// Retrieves a single warehouse item transaction by its ID.
    /// </summary>
    /// <remarks>Required Roles: Any</remarks>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<WarehouseItemTransactionDTO>>> GetById(Guid id)
        => Ok(await _warehouseItemTransactionsService.GetById(id));

    /// <summary>
    /// Creates a new warehouse item transaction.
    /// </summary>
    /// <remarks>Required Roles: Any</remarks>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Response<WarehouseItemTransactionDTO>>> Create([Required][FromBody] WarehouseItemTransactionFormDTO form)
        => Ok(await _warehouseItemTransactionsService.Create(form));

    /// <summary>
    /// Updates an existing warehouse item transaction by its ID.
    /// </summary>
    /// <remarks>Required Roles: Any</remarks>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [Required][FromBody] WarehouseItemTransactionUpdateDTO update)
    {
        update.Id = id;
        await _warehouseItemTransactionsService.Update(update);
        return NoContent(); // 204 No Content is more appropriate for updates
    }

    /// <summary>
    /// Deletes a warehouse item transaction by its ID.
    /// </summary>
    /// <remarks>Required Roles: Any</remarks>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _warehouseItemTransactionsService.Delete(id);
        return NoContent(); // 204 No Content is more appropriate for deletes
    }

    #endregion
}
