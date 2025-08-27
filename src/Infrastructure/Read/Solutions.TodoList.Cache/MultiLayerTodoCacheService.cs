using Solutions.TodoList.Application.Contracts.Read.Cache;
using Solutions.TodoList.Application.Contracts.Read.Projections;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Cache;

public class MultiLayerTodoCacheService(IInMemoryTodoCache hotCache, IMaterializedTodoViewReader coldCache)
    : ITodoCacheService
{
    public async Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = hotCache.Get(id);
        if (todo != null) return todo;

        todo = await coldCache.GetByIdAsync(id, cancellationToken);
        if (todo != null)
            hotCache.Set(todo);

        return todo;
    }

    public async Task<IReadOnlyList<Todo>> ListAsync(string? search, string? sort, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await coldCache.ListAsync(search, sort, skip, take, cancellationToken);
    }

    public Task InvalidateAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        hotCache.Remove(todoId);
        return Task.CompletedTask;
    }

    public Task InvalidateManyAsync(IEnumerable<Guid> todoIds, CancellationToken cancellationToken = default)
    {
        hotCache.RemoveMany(todoIds);
        return Task.CompletedTask;
    }
}