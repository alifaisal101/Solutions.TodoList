using Microsoft.Extensions.Caching.Memory;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Cache;

public class InMemoryTodoCache(IMemoryCache cache)
{
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public Todo? Get(Guid id) => cache.TryGetValue(id, out Todo? todo) ? todo : null;

    public void Set(Todo todo) =>
        cache.Set(todo.Id, todo, _ttl);

    public void Remove(Guid id) =>
        cache.Remove(id);

    public void RemoveMany(IEnumerable<Guid> ids)
    {
        foreach (var id in ids)
            cache.Remove(id);
    }
}