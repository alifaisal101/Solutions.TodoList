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
        var repoTypes = typeof(DatabaseContext).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Select(t => new
            {
                ServiceInterfaces = t.GetInterfaces().Where(i 
                    => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRepository<>)).ToList(),
                Type = t
            })
            .Where(x => x.ServiceInterfaces.Count != 0);

        foreach (var x in repoTypes)
            foreach (var i in x.ServiceInterfaces) services.AddScoped(i, x.Type);
    }
}