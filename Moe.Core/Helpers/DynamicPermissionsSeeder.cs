using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moe.Core.ActionFilters;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Models.Entities;

namespace Moe.Core.Helpers;

public class DynamicPermissionsSeeder
{
    private readonly MasterDbContext _dbContext;
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

    public DynamicPermissionsSeeder(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
    }

    public async Task SeedPermissions()
    {
        var permissions = AllPermissions();
    
        foreach (var permission in permissions)
        {
            foreach (var action in permission.Actions)
            {
                var fullName = $"{permission.Subject.ToKebabCase()}.{action.ToKebabCase()}";
            
                var permissionInDb = await _dbContext.Permissions
                    .FirstOrDefaultAsync(p => p.FullName == fullName);
                
                if (permissionInDb == null)
                {
                    _dbContext.Permissions.Add(new Permission
                    {
                        Subject = permission.Subject.ToKebabCase(),
                        Action = action.ToKebabCase(),
                        FullName = fullName,
                    });
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }


    public List<ShapedPermissions> AllPermissions()
    {
        var groupedPermissions = _actionDescriptorCollectionProvider.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Where(descriptor => HasAuthorizeActionFilter(descriptor))
            .GroupBy(descriptor => descriptor.ControllerName)
            .Select(group => new ShapedPermissions
            {
                Subject = group.Key.ToKebabCase(),
                Actions = group.Select(descriptor => $"{descriptor.ActionName}").Distinct(),
            })
            .OrderBy(controller => controller.Subject)
            .ToList();

        return groupedPermissions;
    }
    
    private bool HasAuthorizeActionFilter(ControllerActionDescriptor descriptor)
    {
        return descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(ServiceFilterAttribute), true)
            .Concat(descriptor.MethodInfo.GetCustomAttributes(typeof(ServiceFilterAttribute), true))
            .Any(attr => attr is ServiceFilterAttribute serviceFilterAttr &&
                         serviceFilterAttr.ServiceType == typeof(AdminDynamicAuthActionFilter));
    }

    public class ShapedPermissions
    {
        public string Subject { get; set; }
        public IEnumerable<string> Actions { get; set; }
    }
}
