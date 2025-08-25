using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Cache;

public interface ITodoCacheService
{
    Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Todo>> ListAsync(
        string? search, string? sort, int skip, int take, CancellationToken cancellationToken = default);

    Task InvalidateAsync(Guid todoId, CancellationToken cancellationToken = default);
    Task InvalidateManyAsync(IEnumerable<Guid> todoIds, CancellationToken cancellationToken = default);
}