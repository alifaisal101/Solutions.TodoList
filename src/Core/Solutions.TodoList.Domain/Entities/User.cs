using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Enums;

namespace Solutions.TodoList.Domain.Entities;

public class User: EntityBase
{
    public string Username { get; private set; }
    public string EncryptedPassword { get; private set; }
    public UserRole Role { get; private set; }

    protected User() { }

    public User(string username, string encryptedPassword, UserRole role)
    {
        Username = username;
        EncryptedPassword = encryptedPassword;
        Role = role;
    }
}