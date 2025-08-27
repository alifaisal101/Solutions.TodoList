using System.Diagnostics;
using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Read.Cache;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Application.Features.Todos.Queries.GetTodoById;

public class GetTodoByIdQueryHandler(
    ITodoCacheService cache,
    ITodoRepository repo,
    ICurrentUserService currentUser)
    : IRequestHandler<GetTodoByIdQuery, ApiResponse<TodoDto>>
{

    public async Task<ApiResponse<TodoDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (userId == null)
            return new ApiResponse<TodoDto>(null, null, false, "Unauthorized");

        var cached = await cache.GetByIdAsync(request.Id, cancellationToken);
        if (cached != null)
        {
            return cached.UserId != userId.Value 
                ? new ApiResponse<TodoDto>(null, null, false, "Resource not found") 
                : new ApiResponse<TodoDto>(ToDto(cached), new Meta(ActivityTraceId()), true, null);
        }

        var entity = await repo.GetByIdForUserAsync(request.Id, userId.Value);
        return entity == null 
            ? new ApiResponse<TodoDto>(null, null, false, "Resource not found") 
            : new ApiResponse<TodoDto>(ToDto(entity), new Meta(ActivityTraceId()), true, null);
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

    private static string ActivityTraceId() => Activity.Current?.Id ?? Guid.NewGuid().ToString();
}