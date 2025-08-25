using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Solutions.TodoList.Application.Contracts.Cache;

namespace Solutions.TodoList.Persistence.Outbox;

public class OutboxWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var cache = scope.ServiceProvider.GetRequiredService<ITodoCacheService>();

            var messages = await db.OutboxMessages
                .Where(x => !x.Processed)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var msg in messages)
            {
                if (msg.Type is "TodoUpdated" or "TodoMarkedDone")
                {
                    var todoId = Guid.Parse(msg.Payload);
                    await cache.InvalidateAsync(todoId, stoppingToken);
                }

                msg.Processed = true;
                msg.ProcessedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}