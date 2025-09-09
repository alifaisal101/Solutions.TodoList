using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Application.Features.Todos.Commands.UpdateTodo;

/// <summary>
/// Update (replace) a todo.
/// </summary>
public record UpdateTodoCommand(Guid Id, TodoTitle Title, TodoDescription Description) : IRequest<ApiResponse<TodoDto>>;
