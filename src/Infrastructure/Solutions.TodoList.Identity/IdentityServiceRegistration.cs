using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Identity.Services;
using Solutions.TodoList.Identity.Settings;

namespace Solutions.TodoList.Identity;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddIdentityExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
#if NET6_0_OR_GREATER
            .ValidateOnStart()
#endif
            ;

        services.AddSingleton<IValidateOptions<JwtSettings>, JwtSettingsValidator>();
        services.AddSingleton<ITokenService>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<JwtSettings>>().Value;

            if (opts.UseRsa)
                return ActivatorUtilities.CreateInstance<RsaJwtTokenService>(sp);
            
            return ActivatorUtilities.CreateInstance<JwtTokenService>(sp);
        });

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}