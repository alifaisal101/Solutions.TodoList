namespace Solutions.TodoList.WebApi.Auth;

public record AuthResponse(Guid Id, string Username, string Role, string AccessToken);
