using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Models.Entities;
using Moe.Core.Null;

namespace Moe.Core.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseErrResponseExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                if (exception is ErrResponseException responseException)
                {
                    context.Response.StatusCode = responseException.StatusCode;
                    context.Response.ContentType = "application/json";
                    var responseJson = responseException.ToJson(context);
                    await context.Response.WriteAsync(responseJson);
                }
            });
        });

        return app;
    } 

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (ctx, next) =>
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAuthorizeData>() == null)
            {
                await next.Invoke();
                return;
            }
            
            if (ctx.User.Identity.IsAuthenticated)
            {
                var dbContext = ctx.RequestServices.GetRequiredService<MasterDbContext>();

                var userId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await dbContext.Users.Where(e => !e.IsDeleted).FirstOrDefaultAsync(e => e.Id == userId);

                if (user == null)
                    ErrResponseThrower.Unauthorized();

                if (user.IsBanned == UserState.Band)
                {
                    ErrResponseThrower.Unauthorized("You are not Active.");

                }



            }
            else
            {
                
                ErrResponseThrower.Unauthorized();
            }

            await next.Invoke();
        });

        return app;
    }

    
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/Main/swagger.json", "v1");
            opt.RoutePrefix = string.Empty;
            
            opt.InjectStylesheet("/swagger/swagger-dark.css");
            opt.InjectJavascript("/swagger/theme-switcher.js");
            
            opt.DocumentTitle = "API Document Title :>";
            opt.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            opt.DisplayRequestDuration();
            
            opt.EnableFilter();
            opt.EnableValidator();
            opt.EnableDeepLinking();
            opt.EnablePersistAuthorization();
            opt.EnableTryItOutByDefault();
            
        });

        return app;
    }
}
