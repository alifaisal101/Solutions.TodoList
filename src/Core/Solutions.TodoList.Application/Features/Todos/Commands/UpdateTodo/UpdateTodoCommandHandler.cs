using System.Diagnostics;
using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Features.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandHandler(
    ITodoRepository repo,
    ICurrentUserService user)
    : IRequestHandler<UpdateTodoCommand, ApiResponse<TodoDto>>
{

    public async Task<ApiResponse<TodoDto>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = user.UserId;
        if (userId == null)
            return new ApiResponse<TodoDto>(null, null, false, "Unauthorized");

        var existing = await repo.GetByIdForUserAsync(request.Id, userId.Value);
        if (existing == null)
            return new ApiResponse<TodoDto>(null, null, false, "Resource not found");

        existing.Update(request.Title.Trim(), request.Description?.Trim() ?? string.Empty);
        var updated = await repo.UpdateAsync(existing);

        var dto = ToDto(updated);
        var meta = new Meta(Activity.Current?.Id ?? Guid.NewGuid().ToString());
        return new ApiResponse<TodoDto>(dto, meta, true, null);
    }

    private static TodoDto ToDto(Todo t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Done = t.Done,
        CreatedAtUtc = t.CreatedAtUtc,
        UpdatedAtUtc = t.UpdatedAtUtc,
        CompletedAtUtc = t.CompletedAtUtc
    };
}