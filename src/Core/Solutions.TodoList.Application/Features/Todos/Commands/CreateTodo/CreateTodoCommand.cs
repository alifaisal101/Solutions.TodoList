using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Application.Features.Todos.Commands.CreateTodo;

/// <summary>
/// Command to create a todo.
/// </summary>
public record CreateTodoCommand(TodoTitle Title, TodoDescription Description) : IRequest<ApiResponse<TodoDto>>;