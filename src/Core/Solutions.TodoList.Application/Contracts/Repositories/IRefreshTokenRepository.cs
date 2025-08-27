using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Repositories;

public interface IRefreshTokenRepository : IAsyncRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId);
    Task RevokeAsync(RefreshToken token);
    Task RevokeAllForUserAsync(Guid userId);
    Task DeleteExpiredAsync(DateTime utcNow);
}