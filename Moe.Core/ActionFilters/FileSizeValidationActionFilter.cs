using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Moe.Core.ActionFilters;

public class FileSizeValidationActionFilter : IActionFilter
{
    private const int _maxSize = 10048576;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Form.Files.Any())
        {
            context.Result = new BadRequestObjectResult("No file uploaded.");
            return;
        }

        var file = context.HttpContext.Request.Form.Files[0];
        if (file.Length > _maxSize)
        {
            context.Result = new BadRequestObjectResult($"File size exceeds {_maxSize / 1024} KB.");
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}