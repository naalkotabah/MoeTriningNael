using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;
using Moe.Core.Models.Statistics;

namespace Moe.Core.Controllers;

#if true
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class StatisticsController : BaseController
{
    private readonly MasterDbContext _context;

    public StatisticsController(MasterDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "super-admin")]
    [HttpGet("users")]
    public async Task<ActionResult<Response<BaseStatisticsDTO<User>>>> Users([FromQuery] BaseStatisticsFilter filter)
    {
        var s = new BaseStatisticsDTO<User>();
        await s.AutoFill(_context, filter);

        return Ok(new Response<BaseStatisticsDTO<User>>(s, null, 200));
    }
}
#endif