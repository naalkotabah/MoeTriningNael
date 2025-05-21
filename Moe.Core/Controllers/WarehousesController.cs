using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Controllers;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Services;


public class WarehousesController : BaseController
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    /// <summary>
    /// Create a new repository with the ability to assign Admins
    /// </summary>
    [Authorize(Roles = "super-admin")]
    [HttpPost]
    public async Task<ActionResult<Response<WarehouseDTO>>> Create([FromBody] WarehouseFormDTO form)
    {
        var result = await _warehouseService.Create(form);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    ///All warehouses with managers
    /// </summary>
    /// 
    [Authorize(Roles = "super-admin")]
    [HttpGet]
    public async Task<ActionResult<Response<List<WarehouseDTO>>>> GetAll()
    {
        var result = await _warehouseService.GetAll();
        return StatusCode(result.StatusCode, result);
    }




    [Authorize(Roles = "super-admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<WarehouseDTO>>> GetById(Guid id) =>
    StatusCode((await _warehouseService.GetById(id)).StatusCode, await _warehouseService.GetById(id));


    [Authorize(Roles = "super-admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<WarehouseDTO>>> Update(Guid id, [FromBody] WarehouseUpdateFormDTO dto)
    {
    
        var result = await _warehouseService.Update(id, dto);
        return StatusCode(result.StatusCode, result);
    }
    [Authorize(Roles = "super-admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<string>>> Delete(Guid id) =>
        StatusCode((await _warehouseService.Delete(id)).StatusCode, await _warehouseService.Delete(id));




    [Authorize(Roles = "super-admin")]
    [HttpPost("{warehouseId}/admins")]
    public async Task<ActionResult<Response<UserDTO>>> CreateAdmin(Guid warehouseId, [FromBody] WarehouseAdminFormDTO form)
    {
        var result = await _warehouseService.CreateAndAssignAdmin(warehouseId, form);
        return StatusCode(result.StatusCode, result);
    }


    [Authorize(Roles = "super-admin")]
    [HttpDelete("{warehouseId}/admins/{userId}")]
    public async Task<ActionResult<Response<string>>> RemoveAdmin(Guid warehouseId, Guid userId)
    {
        var result = await _warehouseService.RemoveAdminFromWarehouse(warehouseId, userId);
        return StatusCode(result.StatusCode, result);
    }


}
