using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using global::Moe.Core.Models.DTOs.Warehouse;
using global::Moe.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Services;
using System;
using System.Threading.Tasks;
using Moe.Core.Helpers;
namespace Moe.Core.Controllers
{

    public class InventoryMovementsController : BaseController
    {
        private readonly IInventoryMovementService _inventoryMovementService;

        public InventoryMovementsController(IInventoryMovementService inventoryMovementService)
        {
            _inventoryMovementService = inventoryMovementService;
        }

 
        [HttpPost("transfer-product")]
        public async Task<ActionResult<Response<string>>> TransferProductBetweenWarehouses([FromBody] InventoryMovementFormDTO form)
        {
            return Ok(await _inventoryMovementService.TransferProductBetweenWarehouses(form));
        }


    }


}
