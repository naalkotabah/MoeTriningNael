using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Moe.Core.Swagger;

public class AcceptLangHeaderSwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        // Add the Accept-Language header parameter
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny>()
                {
                    new OpenApiString("en"),
                    new OpenApiString("ar"),
                    new OpenApiString("ku"),
                },
                Default = new OpenApiString("en")
            }
        });
    }    
}