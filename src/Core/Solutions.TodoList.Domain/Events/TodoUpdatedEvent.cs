using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Events;

public record TodoUpdatedEvent(Guid TodoId) : DomainEvent;