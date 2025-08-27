using MediatR;
using Solutions.TodoList.Application.Common;

namespace Solutions.TodoList.Application.Features.Todo.Commands.DeleteTodo;

/// <summary>
/// Hard delete a todo.
/// </summary>
public record DeleteTodoCommand(Guid Id) : IRequest<ApiResponse<object>>;