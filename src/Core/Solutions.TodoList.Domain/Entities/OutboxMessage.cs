using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Entities;

public class OutboxMessage: EntityBase
{
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
}