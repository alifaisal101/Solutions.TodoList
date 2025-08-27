using Solutions.TodoList.Identity;
using Solutions.TodoList.Persistence;
using Solutions.TodoList.Security;
using Solutions.TodoList.WebApi.Extensions;

namespace Solutions.TodoList.WebApi;

/// <summary>
/// Encapsulates service registration and middleware configuration.
/// Designed so Program.cs remains very small and unit tests can instantiate Startup if needed.
/// </summary>
public class Startup(IConfiguration conf, IHostEnvironment env)
{
    private IConfiguration Conf { get; } = conf;
    private IHostEnvironment Env { get; } = env;

    /// <summary>
    /// Register services to the container.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddPresentation();

        services.AddOpenApi();
        services.AddSwaggerWithJwt();

        services.AddApplication();

        services.AddInfrastructure();
        services.AddReadAndCache();
        services.AddPersistence(Conf);
        services.AddSecurity(Conf);
        services.AddIdentityExtensions(Conf);

        services.AddHttpContextAccessor();
    }

    /// <summary>
    /// Configure HTTP request pipeline.
    /// </summary>
    public void Configure(WebApplication app)
    {
        if (Env.IsDevelopment())
            app.MapOpenApi();

        app.UseCommonMiddleware();

        app.MapControllers();

        app.MapGet("/api/v1/health", () => Results.Ok(new { status = "Healthy" }));
    }
}
