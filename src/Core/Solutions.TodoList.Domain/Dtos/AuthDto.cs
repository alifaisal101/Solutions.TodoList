namespace Solutions.TodoList.Domain.Dtos;

public class AuthDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}