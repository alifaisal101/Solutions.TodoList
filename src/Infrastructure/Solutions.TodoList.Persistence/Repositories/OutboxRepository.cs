using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Persistence.Repositories;

public class OutboxRepository(DatabaseContext context) : BaseRepository<OutboxMessage>(context), IOutboxRepository
{
    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int take)
    {
        return await context.OutboxMessages
            .Where(m => !m.Processed)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(take)
            .ToListAsync();
    }

    public async Task MarkProcessedAsync(Guid outboxId)
    {
        var m = await context.OutboxMessages.FindAsync(new object[] { outboxId });
        if (m == null) return;

        m.Processed = true;
        m.ProcessedOnUtc = DateTime.UtcNow;
        context.OutboxMessages.Update(m);
        await context.SaveChangesAsync();
    }
}