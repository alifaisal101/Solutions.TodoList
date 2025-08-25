namespace Solutions.TodoList.Domain.Common;

public abstract class AggregateRoot : EntityBase
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public DateTimeOffset CreatedAtUtc { get; protected set; }
    public DateTimeOffset? UpdatedAtUtc { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAtUtc { get; protected set; }

    public byte[]? RowVersion { get; protected set; }

    protected void SetCreated(DateTimeOffset at, Guid? by = null)
    {
        CreatedAtUtc = at;
        CreatedBy = by;
    }

    protected void SetUpdated(DateTimeOffset at, Guid? by = null)
    {
        UpdatedAtUtc = at;
        UpdatedBy = by;
    }

    protected void SetDeleted(DateTimeOffset at)
    {
        IsDeleted = true;
        DeletedAtUtc = at;
    }
}