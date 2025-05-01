using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Extensions;

namespace Moe.Core.ActionFilters;

public class AdminDynamicAuthActionFilter : IAsyncActionFilter
{
    private readonly MasterDbContext _dbContext;

    public AdminDynamicAuthActionFilter(MasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null)
        {
            await next();
            return;
        }

        var user = context.HttpContext.User;
    
        if (user.Identity.IsAuthenticated && user.FindFirstValue(ClaimTypes.Role) == "admin")
        {
            string controllerName = controllerActionDescriptor.ControllerName;
            string actionName = controllerActionDescriptor.ActionName;

            string requiredPermission = $"{controllerName.ToKebabCase()}.{actionName.ToKebabCase()}";

            var hasPermission = await HasPermission(user, requiredPermission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }

    private async Task<bool> HasPermission(ClaimsPrincipal user, string permission)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = user.FindFirstValue("DynamicRole"); //TODO-CONSIDER

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return false;
        }

        var hasPermission = await _dbContext.Roles
            .AsNoTracking()
            .Where(r => r.Name == role)
            .SelectMany(r => r.Permissions)
            .AnyAsync(p => p.FullName == permission);
        return hasPermission;
    }
    
    private string GetCrudType(ControllerActionDescriptor descriptor) =>
        descriptor.ActionName.ToKebabCase();
}