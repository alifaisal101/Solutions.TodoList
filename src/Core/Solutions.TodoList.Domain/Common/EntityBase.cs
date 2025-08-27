namespace Solutions.TodoList.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset CreatedAtUtc { get; protected set; }
    public DateTimeOffset? UpdatedAtUtc { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAtUtc { get; protected set; }

    public byte[]? RowVersion { get; protected set; }
}