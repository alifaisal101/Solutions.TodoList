using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Persistence.Repositories;

public class TodoRepository(DatabaseContext context) : BaseRepository<Todo>(context), ITodoRepository
{
    public async Task<Todo?> GetByIdForUserAsync(Guid id, Guid userId)
    {
        return await context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<IReadOnlyList<Todo>> ListByUserAsync(Guid userId, string? search, string? sort, int skip, int take)
    {
        var q = context.Todos.Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(t => EF.Functions.ILike(t.Title, $"%{s}%") 
                             || EF.Functions.ILike(t.Description, $"%{s}%"));
        }

        q = (sort == "createdAt_desc")
            ? q.OrderByDescending(t => t.CreatedAtUtc)
            : q.OrderBy(t => t.CreatedAtUtc);

        return await q.Skip(skip).Take(take).ToListAsync();
    }

    public async Task BatchInsertAsync(IEnumerable<Todo> todos)
    {
        var list = todos as IList<Todo> ?? todos.ToList();
        if (!list.Any()) return;

        await context.Todos.AddRangeAsync(list);
        await context.SaveChangesAsync();
    }

    public async Task BatchUpdateAsync(IEnumerable<Todo> todos)
    {
        var list = todos as IList<Todo> ?? todos.ToList();
        if (!list.Any()) return;

        foreach (var t in list)
            context.Entry(t).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task BatchMarkDoneAsync(IEnumerable<Guid> todoIds)
    {
        var ids = todoIds as IList<Guid> ?? todoIds.ToList();
        if (!ids.Any()) return;

        var todos = await context.Todos.Where(t => ids.Contains(t.Id)).ToListAsync();
        if (todos.Count == 0) return;

        foreach (var t in todos)
        {
            t.MarkDone();
            context.Entry(t).State = EntityState.Modified;
        }

        await context.SaveChangesAsync();
    }

    public async Task BatchDeleteAsync(IEnumerable<Guid> todoIds)
    {
        var ids = todoIds as IList<Guid> ?? todoIds.ToList();
        if (!ids.Any()) return;

        var todos = await context.Todos.Where(t => ids.Contains(t.Id)).ToListAsync();
        if (todos.Count == 0) return;

        context.Todos.RemoveRange(todos);
        await context.SaveChangesAsync();
    }
}