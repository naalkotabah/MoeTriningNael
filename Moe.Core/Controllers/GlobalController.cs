using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moe.Core.ActionFilters;
using Moe.Core.Data;
using Moe.Core.Helpers;

namespace Moe.Core.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class GlobalController : BaseController
{
    private readonly MasterDbContext _context;

    public GlobalController(MasterDbContext context)
    {
        _context = context;
    }

    [HttpGet("is-email-taken/{email}")]
    public async Task<ActionResult<Response<bool>>> IsEmailTaken(string email)
    {
        bool x = await _context.Users.AnyAsync(e => e.Email == email);
        return Ok(x);
    }
    [HttpGet("is-phone-taken/{phone}")]
    public async Task<ActionResult<Response<bool>>> IsPhoneTaken(string phone)
    {
        bool x = await _context.Users.AnyAsync(e => e.PhoneCountryCode+e.Phone == phone);
        return Ok(x);
    }

    [HttpGet("enums")]
    //TODO-CONSIDER response type
    public Dictionary<string, dynamic> GetEnums()
    {
        var enums = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsEnum)
            .ToList();

        var result = new Dictionary<string, dynamic>();

        foreach (var e in enums)
        {
            var values = Enum.GetValues(e);
            var valuesList = new List<dynamic>();
            foreach (var value in values)
            {
                valuesList.Add(new
                {
                    Name = Enum.GetName(e, value),
                    Value = Convert.ToInt32(value) // Use Convert.ToInt32 to get the actual enum value
                });
            }

            try
            {
                result.Add(e.Name, valuesList);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        return result;
    }

}
