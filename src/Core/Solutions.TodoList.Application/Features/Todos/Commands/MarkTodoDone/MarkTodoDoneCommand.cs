using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.Todos.Commands.MarkTodoDone;

/// <summary>
/// Mark a todo done or undone.
/// </summary>
public record MarkTodoDoneCommand(Guid Id, bool Done) : IRequest<ApiResponse<TodoDto>>;