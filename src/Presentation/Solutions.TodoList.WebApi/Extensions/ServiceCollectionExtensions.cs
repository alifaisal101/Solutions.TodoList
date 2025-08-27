using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Cache;
using Solutions.TodoList.Projections;

namespace Solutions.TodoList.WebApi.Extensions;

/// <summary>
/// Service collection extension methods used by the WebApi host.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register presentation related services (controllers, JSON options, CORS).
    /// Keep this thin so unit tests can register controllers in isolation.
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

        // Conservative CORS default; override in production configuration
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", builder =>
            {
                builder
                    .WithOrigins("https://localhost:5001")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Solutions.TodoList API",
                Version = "v1",
                Description = "Todo List API - API surface for initial endpoints"
            });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token in the format: Bearer {your token}\n\n"
                              + "If you only paste the token, make sure to include the 'Bearer ' prefix.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            opts.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [ jwtSecurityScheme ] = Array.Empty<string>()
            });

            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                opts.IncludeXmlComments(xmlPath);
        });

        return services;
    }


    /// <summary>
    /// Register application-level dependencies (MediatR, validators).
    /// Implementation should live in the Application project; this just registers by assembly.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR handlers from Application assembly using a marker type
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestsMarker).Assembly));

        // Uncomment when FluentValidation validators are present in the Application assembly:
        // services.AddValidatorsFromAssembly(typeof(RequestsMarker).Assembly);

        return services;
    }

    /// <summary>
    /// Register infrastructure-level dependencies (db, identity, cache, hosted services).
    /// Keep minimal here - actual infrastructure registrations live in AddPersistence or infra projects.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // JWT auth placeholder — configure TokenValidationParameters in real app
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // configure in production: options.Authority / TokenValidationParameters / IssuerSigningKey
        });

        services.AddAuthorization();

        return services;
    }
    
    /// <summary>
    /// Convenience to register both read + cache services.
    /// </summary>
    public static IServiceCollection AddReadAndCache(this IServiceCollection services)
    {
        services.AddCacheServices();
        services.AddReadServices();
        return services;
    }
}
