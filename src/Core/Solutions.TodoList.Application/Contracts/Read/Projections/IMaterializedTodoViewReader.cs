using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Read.Projections;

/// <summary>
/// Abstraction for reading from the materialized todos view.
/// </summary>
public interface IMaterializedTodoViewReader
{
    Task<Todo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Todo>> ListAsync(string? search, string? sort, int skip, int take, CancellationToken ct = default);
}