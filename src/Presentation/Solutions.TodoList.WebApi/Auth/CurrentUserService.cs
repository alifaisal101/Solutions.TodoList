using System.Security.Claims;
using Solutions.TodoList.Application.Contracts.Identity;

namespace Solutions.TodoList.WebApi.Auth;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;
    public Guid? UserId
    {
        get
        {
            var sub = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? Principal?.FindFirst("sub")?.Value;
            if (Guid.TryParse(sub, out var g)) return g;
            return null;
        }
    }

    public string? UserName => Principal?.Identity?.Name;
}
