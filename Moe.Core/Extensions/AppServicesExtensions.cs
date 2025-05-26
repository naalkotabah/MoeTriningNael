using System.Globalization;
using System.Reflection;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Moe.Core.Helpers;
using Moe.Core.Services;
using Moe.Core.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Moe.Core.Extensions;

public static class AppServicesExtensions
{
    public static IServiceCollection AddLocalizationConfig(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(opt =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ar"),
                new CultureInfo("ku")
            };

            opt.DefaultRequestCulture = new RequestCulture("en");
            opt.SupportedCultures = supportedCultures;
        });
        return services;
    }
    
    public static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
        string jwtSignInKey = ConfigProvider.config.GetSection("Jwt:SecretKey").Get<string>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignInKey))
                };
            });
        services.AddAuthorization();
        return services;
    }
    
    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        return services;
    }
    
    public static IServiceCollection AddSignalRConfig(this IServiceCollection services)
    {
        services.AddSignalR();
        //services.AddScoped<INotificationsService, FcmNotificationsService>();
        //services.AddScoped<INotificationsService, OneSignalNotificationsService>();
        services.AddScoped<INotificationsService, SignalRNotificationsService>();
        services.AddScoped<IHelperNotifications, HelperNotifications>();
        
        return services;
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAttachmentsService, AttachmentsService>();
        services.AddScoped<PhoneNumberNormalizer>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ICountriesService, CountriesService>();
        services.AddScoped<ICitiesService, CitiesService>();
        services.AddScoped<IRolesService, RolesService>();
        
        //{{INSERTION_POINT}}
        
        return services;
    }

    public static IServiceCollection AddCustomRouting(this IServiceCollection services)
    {
        services.AddRouting(opt => opt.LowercaseUrls = true);
        return services;
    }
    
    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services.AddControllers(opt =>
        {
            opt.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            opt.OutputFormatters.Add(new StringOutputFormatter());
            opt.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseControllerModelConvention()) );
        });
        return services;
    }
    
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc(
                name: "Main",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Your Swagger Title :>",
                    Description = "Your Swagger Description :>",
                    Contact = new OpenApiContact
                    {
                        Name = "Mohammed Thaier",
                        Email = "devmoethaier@proton.me",
                        Url = new Uri("https://www.linkedin.com/in/mohammed-thaier-421964202/")
                    }
                });
            
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header, \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });
            
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            
            opt.OperationFilter<AcceptLangHeaderSwaggerOperationFilter>();
            opt.OperationFilter<AcceptHeaderOperationFilter>();
        });

        return services;
    }

    public static IServiceCollection AddTmpStuff(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddHangfireConfig(this IServiceCollection services)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(ConfigProvider.config.GetConnectionString("Local")));

        return services;
    }
}
