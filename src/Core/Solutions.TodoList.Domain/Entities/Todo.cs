using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Events;

namespace Solutions.TodoList.Domain.Entities;

public class Todo : EntityBase, IHasDomainEvents
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Done { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public Guid UserId { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];

    protected Todo() { }

    public Todo(Guid userId, string title, string description)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Done = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkDone()
    {
        if (Done) return;
        
        Done = true;
        CompletedAtUtc = DateTime.UtcNow;
        DomainEvents.Add(new TodoMarkedDoneEvent(Id));
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
        UpdatedAtUtc = DateTime.UtcNow;
        DomainEvents.Add(new TodoUpdatedEvent(Id));
    }
}