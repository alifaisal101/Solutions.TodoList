namespace Solutions.TodoList.Application.Contracts.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashed);
}