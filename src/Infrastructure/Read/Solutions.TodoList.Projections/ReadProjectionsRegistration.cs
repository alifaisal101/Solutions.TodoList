using Microsoft.Extensions.DependencyInjection;
using Solutions.TodoList.Application.Contracts.Read.Projections;

namespace Solutions.TodoList.Projections;

public static class ReadProjectionsRegistration
{
    public static IServiceCollection AddReadServices(this IServiceCollection services)
    {
        services.AddScoped<IMaterializedTodoViewReader, MaterializedTodoViewReader>();
        return services;
    }
}
