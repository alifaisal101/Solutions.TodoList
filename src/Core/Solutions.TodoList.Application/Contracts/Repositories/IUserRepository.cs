using Solutions.TodoList.Domain.Common;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Contracts.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}