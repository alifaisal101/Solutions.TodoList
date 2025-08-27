using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Persistence.Outbox;

namespace Solutions.TodoList.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connStr))
            throw new InvalidOperationException("DefaultConnection is not configured in the configuration source.");
        var migrationsAssembly = typeof(DatabaseContext).Assembly.GetName().Name;
        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(connStr, npgsql => npgsql.MigrationsAssembly(migrationsAssembly)));

        services.AddScoped<IDbConnection>(_ =>
        {
            var connection = new NpgsqlConnection(connStr);
            return connection;
        });

        services.AddReposViaReflection();
        services.AddHostedService<OutboxWorker>();

        return services;
    }

    private static void AddReposViaReflection(this IServiceCollection services)
    {
        var persistenceAsm = typeof(DatabaseContext).Assembly;

        var contractsAsm = typeof(Solutions.TodoList.Application.Contracts.Repositories.IUserRepository).Assembly;
        var domainAsm = typeof(IAsyncRepository<>).Assembly;

        var allowedAssemblies = new[] { contractsAsm, domainAsm };

        var implTypes = persistenceAsm.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false });

        foreach (var implType in implTypes)
        {
            var serviceInterfaces = implType.GetInterfaces()
                .Where(i => i.IsPublic && allowedAssemblies.Contains(i.Assembly))
                .ToList();
            if (serviceInterfaces.Count == 0) continue;
            foreach (var svc in serviceInterfaces.Distinct()) services.AddScoped(svc, implType);
        }
    }
}