using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Persistence.Repositories;

public abstract class BaseRepository<T>(DbContext context) : IAsyncRepository<T>
    where T : class
{
    protected readonly DbContext Context = context;

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await GetByIdInternalAsync(id);

    public virtual async Task<T?> GetByIdAsync(string id)
        => await GetByIdInternalAsync(id);

    protected virtual async Task<T?> GetByIdInternalAsync(object key)
        => await Context.Set<T>().FindAsync(key);

    public virtual async Task<IReadOnlyList<T>> ListAllAsync()
        => await Context.Set<T>().ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();

        await OnAddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();

        await OnUpdateAsync(entity);
        return entity;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Context.Set<T>().Remove(entity);
        await OnDeleteAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListBatchAsync(int skip, int take)
        => await Context.Set<T>().Skip(skip).Take(take).ToListAsync();

    protected virtual Task OnAddAsync(T _) => Task.CompletedTask;
    protected virtual Task OnUpdateAsync(T _) => Task.CompletedTask;
    protected virtual Task OnDeleteAsync(T _) => Task.CompletedTask;
}