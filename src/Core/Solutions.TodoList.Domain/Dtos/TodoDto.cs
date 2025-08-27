namespace Solutions.TodoList.Domain.Dtos;

public class TodoDto
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Done { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
}