namespace Solutions.TodoList.Domain.Common;

public interface IHasDomainEvents
{
    List<DomainEvent> DomainEvents { get; set; }
}