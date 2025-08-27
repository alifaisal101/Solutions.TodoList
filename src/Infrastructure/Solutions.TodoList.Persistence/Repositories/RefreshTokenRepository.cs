using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Persistence.Repositories;

public class RefreshTokenRepository(DatabaseContext context)
    : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.Revoked && rt.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAsync(RefreshToken? token)
    {
        if (token == null) return;
        token.Revoke();
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var tokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.Revoked && rt.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync();

        if (tokens.Count == 0) return;

        foreach (var t in tokens) t.Revoke();
        context.RefreshTokens.UpdateRange(tokens);
        await context.SaveChangesAsync();
    }

    public async Task DeleteExpiredAsync(DateTime utcNow)
    {
        var expired = await context.RefreshTokens
            .Where(rt => rt.ExpiresAtUtc <= utcNow)
            .ToListAsync();

        if (expired.Count == 0) return;

        context.RefreshTokens.RemoveRange(expired);
        await context.SaveChangesAsync();
    }
    
    public async Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken)
        => await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);
}