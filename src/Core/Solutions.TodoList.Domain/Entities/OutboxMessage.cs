using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Entities;

public class OutboxMessage: EntityBase
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
}