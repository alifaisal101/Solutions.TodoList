using System.Diagnostics;
using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Domain.Events;

namespace Solutions.TodoList.Application.Features.Todos.Commands.MarkTodoDone;

public class MarkTodoDoneCommandHandler(
    ITodoRepository repo,
    ICurrentUserService user)
    : IRequestHandler<MarkTodoDoneCommand, ApiResponse<TodoDto>>
{
    public async Task<ApiResponse<TodoDto>> Handle(MarkTodoDoneCommand request, CancellationToken cancellationToken)
    {
        var userId = user.UserId;
        if (userId == null)
            return new ApiResponse<TodoDto>(null, null, false, "Unauthorized");

        var todo = await repo.GetByIdForUserAsync(request.Id, userId.Value);
        if (todo == null)
            return new ApiResponse<TodoDto>(null, null, false, "Resource not found");

        if (request.Done)
            todo.MarkDone();
        else
        {
            if (todo.Done)
            {
                typeof(Todo).GetProperty("Done")?.SetValue(todo, false);
                typeof(Todo).GetProperty("CompletedAtUtc")?.SetValue(todo, null);
                todo.DomainEvents.Add(new TodoUpdatedEvent(todo.Id));
            }
        }

        var updated = await repo.UpdateAsync(todo);

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