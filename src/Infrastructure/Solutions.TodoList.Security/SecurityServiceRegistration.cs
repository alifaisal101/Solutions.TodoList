using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solutions.TodoList.Application.Contracts.Security;

namespace Solutions.TodoList.Security;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<Argon2Settings>()
            .Bind(configuration.GetSection("Argon2"))
            .ValidateDataAnnotations()
#if NET6_0_OR_GREATER
            .ValidateOnStart()
#endif
            ;

        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        return services;
    }
}