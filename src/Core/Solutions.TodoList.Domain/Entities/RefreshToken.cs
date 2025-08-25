using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Entities;

public class RefreshToken: EntityBase
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool Revoked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        Revoked = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Revoke() => Revoked = true;
}