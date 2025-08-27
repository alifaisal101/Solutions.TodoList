using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Identity;

public interface ITokenService
{
    (string accessToken, string refreshToken) CreateTokens(User user);
    string HashRefreshToken(string refreshToken);
}