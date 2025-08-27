using Microsoft.EntityFrameworkCore;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Persistence.Repositories;

public class UserRepository(DatabaseContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}