using System.Diagnostics;
using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todos.Queries.ListTodos;

public class ListTodosQueryHandler(
    ITodoRepository repo,
    ICurrentUserService currentUser)
    : IRequestHandler<ListTodosQuery, ApiResponse<PagedResult<TodoDto>>>
{
    public async Task<ApiResponse<PagedResult<TodoDto>>> Handle(ListTodosQuery r, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (userId == null)
            return new ApiResponse<PagedResult<TodoDto>>(null, null, false, "Unauthorized");

        var todos = await repo.ListByUserAsync(userId.Value, r.Search, r.Sort, r.Skip, r.Take);
        var totalCount = await repo.CountByUserAsync(userId.Value, r.Search);

        var dtoList = todos.Select(t => new TodoDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Done = t.Done,
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc,
            CompletedAtUtc = t.CompletedAtUtc
        }).ToList();

        var paged = new PagedResult<TodoDto>(dtoList, totalCount);
        var meta = new Meta(ActivityTraceId(), PageFromSkip(r.Skip, r.Take), r.Take, totalCount);

        return new ApiResponse<PagedResult<TodoDto>>(paged, meta, true, null);
    }

    private static int PageFromSkip(int skip, int take) => (take <= 0) ? 1 : (skip / take) + 1;
    private static string ActivityTraceId() => Activity.Current?.Id ?? Guid.NewGuid().ToString();
}
