using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Features.Todos.Commands.CreateTodo;

public class CreateTodoCommandHandler(
    ITodoRepository repo,
    ICurrentUserService user,
    ILogger<CreateTodoCommandHandler> logger)
    : IRequestHandler<CreateTodoCommand, ApiResponse<TodoDto>>
{
    public async Task<ApiResponse<TodoDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = user.UserId;
        if (userId == null)
            return new ApiResponse<TodoDto>(null, null, false, "Unauthorized");

        var todo = new Todo(userId.Value, request.Title.Trim(), request.Description?.Trim() ?? string.Empty);

        var created = await repo.AddAsync(todo);

        var dto = ToDto(created);
        var meta = new Meta(Activity.Current?.Id ?? Guid.NewGuid().ToString());
        return new ApiResponse<TodoDto>(dto, meta);
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