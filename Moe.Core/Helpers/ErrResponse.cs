using System.Text.Json;
using Moe.Core.Translations;

namespace Moe.Core.Null;

public class ErrResponseException : Exception
{
    public int StatusCode { get; set; }
    public string MsgKey { get; set; }
    
    public ErrResponseException(string msgKey, int statusCode)
    {
        MsgKey = msgKey;
        StatusCode = statusCode;
    }


    public string ToJson(HttpContext context)
    {
        var lang = context.Request.Headers["Accept-Language"].ToString().ToLowerInvariant();
        
        return JsonSerializer.Serialize(new
        {
            statusCode = StatusCode,
            msg = Localizer.Translate(MsgKey, lang)
        });
    }
}

public static class ErrResponseThrower
{
    public static void BadRequest(string msgKey = "BAD_REQUEST")
    {
        throw new ErrResponseException(msgKey, 400);
    }
    public static void Unauthorized(string msgKey = "UNAUTHORIZED")
    {
        throw new ErrResponseException(msgKey, 401);
    }
    public static void Forbidden(string msgKey = "FORBIDDEN")
    {
        throw new ErrResponseException(msgKey, 403);
    }
    public static void NotFound(string msgKey = "NOT_FOUND")
    {
        throw new ErrResponseException(msgKey, 404);
    }
    public static void Conflict(string msgKey = "CONFLICT")
    {
        throw new ErrResponseException(msgKey, 409);
    }
    
    public static void InternalServerErr(string msgKey = "INTERNAL_SERVER_ERROR")
    {
        throw new ErrResponseException(msgKey, 500);
    }
}