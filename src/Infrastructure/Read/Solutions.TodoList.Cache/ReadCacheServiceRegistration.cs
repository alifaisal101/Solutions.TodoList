using Microsoft.Extensions.DependencyInjection;
using Solutions.TodoList.Application.Contracts.Read.Cache;

namespace Solutions.TodoList.Cache;

public static class ReadCacheServiceRegistration
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IInMemoryTodoCache, InMemoryTodoCache>();
        services.AddScoped<ITodoCacheService, MultiLayerTodoCacheService>();

        return services;
    }
}