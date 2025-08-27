using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Repositories;

public interface ITodoRepository : IAsyncRepository<Todo>
{
    Task<Todo?> GetByIdForUserAsync(Guid id, Guid userId);
    Task<IReadOnlyList<Todo>> ListByUserAsync(Guid userId, string? search, string? sort, int skip, int take);
    Task BatchInsertAsync(IEnumerable<Todo> todos);
    Task BatchUpdateAsync(IEnumerable<Todo> todos);
    Task BatchMarkDoneAsync(IEnumerable<Guid> todoIds);
    Task BatchDeleteAsync(IEnumerable<Guid> todoIds);
}