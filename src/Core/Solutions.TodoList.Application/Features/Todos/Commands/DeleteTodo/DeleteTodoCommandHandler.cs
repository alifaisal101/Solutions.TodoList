using System.Diagnostics;
using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;

namespace Solutions.TodoList.Application.Features.Todos.Commands.DeleteTodo;

public class DeleteTodoCommandHandler(
    ITodoRepository repo,
    ICurrentUserService user)
    : IRequestHandler<DeleteTodoCommand, ApiResponse<object>>
{

    public async Task<ApiResponse<object>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = user.UserId;
        if (userId == null)
            return new ApiResponse<object>(null, null, false, "Unauthorized");

        var todo = await repo.GetByIdForUserAsync(request.Id, userId.Value);
        if (todo == null)
            return new ApiResponse<object>(null, null, false, "Resource not found");

        await repo.DeleteAsync(todo);
        var meta = new Meta(Activity.Current?.Id ?? Guid.NewGuid().ToString());
        return new ApiResponse<object>(null, meta, true, null);
    }
}