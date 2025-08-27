using System.Security.Claims;

namespace Solutions.TodoList.Application.Contracts.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    ClaimsPrincipal? Principal { get; }
}