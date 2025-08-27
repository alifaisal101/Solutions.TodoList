using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todo.Commands.UpdateTodo;

/// <summary>
/// Update (replace) a todo.
/// </summary>
public record UpdateTodoCommand(Guid Id, string Title, string Description) : IRequest<ApiResponse<TodoDto>>;
