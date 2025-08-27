using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todo.Commands.CreateTodo;

/// <summary>
/// Command to create a todo.
/// </summary>
public record CreateTodoCommand(string Title, string Description) : IRequest<ApiResponse<TodoDto>>;