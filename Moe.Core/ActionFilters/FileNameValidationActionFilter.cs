using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Moe.Core.ActionFilters;

public class FileNameValidationActionFilter : IActionFilter
{
    private static readonly Regex InvalidFileName = new Regex("[<>:\"/\\|?*]");

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Form.Files.Any())
        {
            return;
        }

        var file = context.HttpContext.Request.Form.Files[0];

        bool isValidName = !InvalidFileName.IsMatch(file.FileName);
        if (!isValidName)
        {
            context.Result = new BadRequestObjectResult("Invalid file name");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}