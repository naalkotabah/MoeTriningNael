using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Models.Entities;
using Moe.Core.Services;
using System;
using System.Threading.Tasks;
namespace Moe.Core.Controllers;
public class WarehouseItemsController : BaseController
{
    private readonly IWarehouseItemService _warehouseItemService;

    public WarehouseItemsController(IWarehouseItemService warehouseItemService)
    {
        _warehouseItemService = warehouseItemService;
    }


    /// <summary>
    /// إضافة أو تحديث منتج في مستودع
    /// POST: /api/WarehouseItems/{warehouseId}/items
    /// </summary>
    /// 
    [Authorize(Roles = "super-admin")]
    [HttpPost("{warehouseId}/items")]
    public async Task<Response<string>> AddProductToWarehouse(Guid warehouseId, [FromBody] WarehouseItemFormDTO form)
    {
        form.WarehouseId = warehouseId;
        return await _warehouseItemService.AddItemToWarehouse(form);
    }


    /// <summary>
    /// حذف منتج من مستودع
    /// DELETE: /api/WarehouseItems/{warehouseId}/items/{itemId}
    /// </summary>
    [Authorize(Roles = "super-admin")]
    [HttpDelete("{warehouseId}/items/{itemId}")]
    public async Task<Response<string>> RemoveProductFromWarehouse(Guid warehouseId, Guid itemId)
    {
        return await _warehouseItemService.RemoveItemFromWarehouse(warehouseId, itemId);
    }
}
