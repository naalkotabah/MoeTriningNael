using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Services;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;

namespace Moe.Core.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    #region CRUDs
    [Authorize(Roles = "super-admin")]
    [TypeFilter(typeof(SoftDeleteAccessFilterActionFilter))]
    [HttpGet]
    public async Task<ActionResult<Response<PagedList<UserDTO>>>> GetAll([FromQuery] UserFilterDTO filter)
    {
        
        return Ok(await _usersService.GetAll(filter));  
    }



    [Authorize(Roles = "super-admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<UserDTO>>> GetById(Guid id) =>
        Ok(await _usersService.GetById(id));



    [Authorize(Roles = "super-admin")]
    [HttpPost]
    public async Task<ActionResult<Response<UserDTO>>> Create([FromBody] UserFormDTO form) =>
        Ok(await _usersService.Create(form));




    [Authorize(Roles = "super-admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<UserDTO>>> Update(Guid id, [FromBody] UserUpdateDTO update)
    {
        await _usersService.Update(id, update);
        return Ok();
    }




    [Authorize(Roles = "super-admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(Guid id ,[FromQuery] bool isPermanent)
    {
        await _usersService.Delete(id, isPermanent);
        return Ok("For you");
    }
    #endregion

    [Authorize(Roles = "super-admin")]
    [HttpPost("ban/{id}")]
    public async Task<IActionResult> BanUser(Guid id)
    {
        await _usersService.BanUser(id);
        return Ok("User has been banned.");
    }



    [HttpGet("current")]
    public async Task<ActionResult<Response<UserDTO>>> GetCurrent() =>
        Ok(await _usersService.GetById(CurId));




    [HttpPut("current")]
    public async Task<ActionResult<Response<UserDTO>>> UpdateCurrent(Guid id ,[FromBody] UserUpdateDTO update)
    {
     
        update.Id = CurId;
        
        await _usersService.Update(CurId, update);
        return Ok(update);
    }
}