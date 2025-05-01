using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moe.Core.Data;
using Moe.Core.Hubs;
using Moe.Core.Models.Entities;

namespace Moe.Core.Controllers;

#if true
/// <summary>
/// Used mainly by backend developers for testing, please don't bother with it
/// </summary>
/// 

[ApiExplorerSettings(IgnoreApi = true)]
public class DummyController : BaseController
{

    [HttpGet("test-hub-injection")]
    public IActionResult TestHubINjection([FromServices] IHubContext<MasterHub> hub)
    {
        return Ok(hub.ToString());
    }
    
    //[Authorize]
    [HttpGet("test-auth")]
    public IActionResult TestAuth()
    {
        return Ok();
    }
    
    [HttpGet("xml")]
    public IActionResult xml()
    {
        return Ok(new Foo());
    }

    [HttpGet("TestingDifferentCasesA")]
    public IActionResult CaseA() => Ok();
    [HttpGet("testing-different-cases-B")]
    public IActionResult CaseB() => Ok();
    [HttpGet("testingDifferentCasesC")]
    public IActionResult CaseC() => Ok();

    [HttpGet("fuck")]
    public IActionResult Fuck() => Ok();


    [HttpGet("test-new-validator")]
    public async Task<IActionResult> TestNewValidator([FromServices] MasterDbContext context, [FromQuery] List<Guid> ids)
    {
        await context.EnsureEntitiesIdsExists<City>(ids);
        return Ok();
    }
}
#endif

public class Foo
{
    public int X { get; set; } = 10;

    public List<string> Y { get; set; } = new List<string>()
    {
        "Hey",
        "There"
    };
}