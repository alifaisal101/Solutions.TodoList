namespace Solutions.TodoList.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAtUtc { get; protected set; }

    public byte[]? RowVersion { get; protected set; }
}