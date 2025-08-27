using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Requests.Todo;

namespace Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchDeleteTodos;

/// <summary>
/// Batch delete todos.
/// </summary>
public record BatchDeleteTodosCommand(Guid[] Ids) : IRequest<ApiResponse<BatchDeleteResponse>>;