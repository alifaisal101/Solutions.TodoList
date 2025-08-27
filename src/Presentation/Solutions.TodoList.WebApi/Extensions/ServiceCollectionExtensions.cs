using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Cache;
using Solutions.TodoList.Identity.Settings;
using Solutions.TodoList.Projections;
using Solutions.TodoList.WebApi.Auth;

namespace Solutions.TodoList.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection AddReadAndCache(this IServiceCollection services)
    {
        services.AddCacheServices();
        services.AddReadServices();
        return services;
    }
    
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Jwt settings are missing from configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // set true in prod
            options.SaveToken = true;

            if (jwt.UseRsa)
            {
                if (string.IsNullOrWhiteSpace(jwt.RsaPublicKeyPem))
                    throw new InvalidOperationException("Jwt:UseRsa = true but Jwt:RsaPublicKeyPem is empty.");

                var rsa = RSA.Create();
                rsa.ImportFromPem(jwt.RsaPublicKeyPem.ToCharArray());
                var rsaKey = new RsaSecurityKey(rsa);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(jwt.SymmetricKey))
                    throw new InvalidOperationException("Jwt:SymmetricKey is not configured.");

                var keyBytes = Encoding.UTF8.GetBytes(jwt.SymmetricKey);
                var signingKey = new SymmetricSecurityKey(keyBytes);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            }

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx => Task.CompletedTask
            };
        });

        services.AddAuthorization();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
