using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchCreateTodos;

/// <summary>
/// Batch create todos.
/// </summary>
public record BatchCreateTodosCommand((TodoTitle Title, TodoDescription Description)[] Items) : IRequest<ApiResponse<IEnumerable<TodoDto>>>;