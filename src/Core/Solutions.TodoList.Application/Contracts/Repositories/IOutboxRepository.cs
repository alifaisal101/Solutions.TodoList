using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Repositories;

public interface IOutboxRepository : IAsyncRepository<OutboxMessage>
{
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int take);
    Task MarkProcessedAsync(Guid outboxId);
}