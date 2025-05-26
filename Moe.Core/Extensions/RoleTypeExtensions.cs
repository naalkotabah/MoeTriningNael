using Moe.Core.Models.Entities;

namespace Moe.Core.Extensions;

public static class RoleTypeExtensions
{
    public static string ToRoleString(this StaticRole staticRole)
    {
        switch (staticRole)
        {
            case StaticRole.SUPER_ADMIN:
                return "super-admin";
            case StaticRole.ADMIN:
                return "admin";
            case StaticRole.NORMAL:
                return "NORMAL";
            case StaticRole.WAREHOUSE_ADMIN:
                return "warehouse-admin";
            default:
                return "UNDEFINED";
        }
    }
}