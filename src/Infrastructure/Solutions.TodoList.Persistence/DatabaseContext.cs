using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Persistence;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Todo> Todos { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<EntityBase>()
            .SelectMany(e =>
            {
                var hasEvents = e.Entity as IHasDomainEvents;
                return hasEvents?.DomainEvents ?? Enumerable.Empty<DomainEvent>();
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                Type = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent),
                OccurredOnUtc = DateTime.UtcNow,
                Processed = false
            });
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in ChangeTracker.Entries<EntityBase>())
        {
            var hasEvents = entity.Entity as IHasDomainEvents;
            hasEvents?.DomainEvents.Clear();
        }

        return result;
    }
}