using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todos.Queries.ListTodos;

/// <summary>
/// Query to list todos with pagination and filtering.
/// </summary>
public record ListTodosQuery(string? Search, string? Sort, int Skip, int Take) : IRequest<ApiResponse<PagedResult<TodoDto>>>;