using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Events;

public record TodoMarkedDoneEvent(Guid TodoId) : DomainEvent;