namespace Solutions.TodoList.Domain.Common;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}