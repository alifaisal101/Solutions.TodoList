using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Enums;

namespace Solutions.TodoList.Domain.Entities;

public class User: EntityBase
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    protected User() { }

    public User(string userName, string email, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}