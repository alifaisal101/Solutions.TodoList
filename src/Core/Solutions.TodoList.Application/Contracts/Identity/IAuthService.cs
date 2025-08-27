using Solutions.TodoList.Application.Requests.Auth;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Contracts.Identity;

public interface IAuthService
{
    Task<AuthDto> RegisterAsync(RegisterRequest request);
    Task<AuthDto> LoginAsync(LoginRequest request);
    Task<AuthDto> RefreshTokenAsync(string refreshToken);
}