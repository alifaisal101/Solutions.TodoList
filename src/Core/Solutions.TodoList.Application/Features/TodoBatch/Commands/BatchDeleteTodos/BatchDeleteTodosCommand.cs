using MediatR;
using Solutions.TodoList.Application.Common;

namespace Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchDeleteTodos;

/// <summary>
/// Batch delete todos.
/// </summary>
public record BatchDeleteTodosCommand(Guid[] Ids) : IRequest<ApiResponse<BatchDeleteResponse>>;