using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchCreateTodos;

/// <summary>
/// Batch create todos.
/// </summary>
public record BatchCreateTodosCommand((string Title, string Description)[] Items) : IRequest<ApiResponse<IEnumerable<TodoDto>>>;