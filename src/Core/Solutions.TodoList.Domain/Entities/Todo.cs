using System.ComponentModel.DataAnnotations.Schema;
using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Events;
using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Domain.Entities;

public class Todo : EntityBase, IHasDomainEvents
{
    public TodoTitle Title { get; private set; }
    public TodoDescription Description { get; private set; }
    public bool Done { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public Guid UserId { get; private set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; set; } = [];

    protected Todo() { }

    public Todo(Guid userId, TodoTitle title, TodoDescription description)
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

    public void Update(TodoTitle title, TodoDescription description)
    {
        Title = title;
        Description = description;
        UpdatedAtUtc = DateTime.UtcNow;
        DomainEvents.Add(new TodoUpdatedEvent(Id));
    }
}