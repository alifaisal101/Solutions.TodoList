using MediatR;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchMarkTodosDone;

/// <summary>
/// Batch mark many todos done/undone.
/// </summary>
public record BatchMarkTodosDoneCommand(Guid[] Ids, bool Done) : IRequest<ApiResponse<IEnumerable<TodoDto>>>;