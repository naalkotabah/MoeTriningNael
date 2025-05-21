using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Controllers;
using Moe.Core.Models.DTOs.Items;
using static ItemService;

[Route("api/items")]
public class ItemsController : BaseController
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [Authorize(Roles = "super-admin,admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemFormDTO form)
    {
        var result = await _itemService.Create(form);
        return StatusCode(result.StatusCode, result);
    }
    [Authorize(Roles = "super-admin,admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ItemFilterDTO filter)
    {
        var result = await _itemService.GetAll(filter);
        return StatusCode(result.StatusCode, result);
    }
    [Authorize(Roles = "super-admin,admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _itemService.GetById(id);
        return StatusCode(result.StatusCode, result);
    }
    [Authorize(Roles = "super-admin,admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ItemFormDTO form)
    {
        var result = await _itemService.Update(id, form);
        return StatusCode(result.StatusCode, result);
    }
    [Authorize(Roles = "super-admin,admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _itemService.Delete(id);
        return StatusCode(result.StatusCode, result);
    }
}
