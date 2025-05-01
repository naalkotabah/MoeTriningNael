using System.Security.Claims;

namespace Moe.Core.Extensions;

public static class HttpContextExtensions
{
    public static string? GetLang(this HttpContext httpContext)
    {
        if (httpContext == null) return null;

        return httpContext.Request.Headers["Accept-Language"].FirstOrDefault()?.ToLower();
    }

    public static Guid? GetCurUserId(this HttpContext httpContext)
    {
        if (httpContext == null) return null;

        // Assuming user ID is stored as a claim in the JWT token
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }
        
        return userId;
    }

}