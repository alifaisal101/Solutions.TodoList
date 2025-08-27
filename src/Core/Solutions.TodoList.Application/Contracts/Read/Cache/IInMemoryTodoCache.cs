using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Read.Cache;

/// <summary>
/// Lightweight wrapper around IMemoryCache used by the cache layer.
/// </summary>
public interface IInMemoryTodoCache
{
    Todo? Get(Guid id);
    void Set(Todo todo);
    void Remove(Guid id);
    void RemoveMany(IEnumerable<Guid> ids);
}