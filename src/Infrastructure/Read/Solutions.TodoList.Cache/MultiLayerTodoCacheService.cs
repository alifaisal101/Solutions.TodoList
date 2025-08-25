using Solutions.TodoList.Application.Contracts.Cache;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Projections;

namespace Solutions.TodoList.Cache;

public class MultiLayerTodoCacheService(InMemoryTodoCache hotCache, MaterializedTodoViewReader coldCache)
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

    public async Task<IReadOnlyList<Todo>> ListAsync(
        string? search, string? sort, int skip, int take, CancellationToken cancellationToken = default)
    {
        // For batch, always use cold cache (materialized view)
        return await coldCache.ListAsync(search, sort, skip, take, cancellationToken);
    }

    public async Task InvalidateAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        // Remove from hot cache
        hotCache.Remove(todoId);
        // Optionally: trigger materialized view refresh (handled by background worker)
        await Task.CompletedTask;
    }

    public async Task InvalidateManyAsync(IEnumerable<Guid> todoIds, CancellationToken cancellationToken = default)
    {
        hotCache.RemoveMany(todoIds);
        await Task.CompletedTask;
    }
}