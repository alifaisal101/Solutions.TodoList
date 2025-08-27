using Solutions.TodoList.Domain.Common;

namespace Solutions.TodoList.Domain.Entities;

public class RefreshToken: EntityBase
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool Revoked { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    protected RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
    {
        UserId = userId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        Revoked = false;
    }

    public void Revoke()
    {
        if (Revoked) return;
        Revoked = true;
        RevokedAtUtc = DateTime.UtcNow;
    }}