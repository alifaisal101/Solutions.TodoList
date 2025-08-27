using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todo.Queries.GetTodoById;

/// <summary>
/// Query to get a todo by id.
/// </summary>
public record GetTodoByIdQuery(Guid Id) : IRequest<ApiResponse<TodoDto>>;