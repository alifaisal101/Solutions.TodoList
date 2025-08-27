using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchCreateTodos;
using Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchDeleteTodos;
using Solutions.TodoList.Application.Features.TodoBatch.Commands.BatchMarkTodosDone;
using Solutions.TodoList.Application.Requests.Todos;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.WebApi.Controllers;

/// <summary>
/// Controller for managing todos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TodosBatchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TodosController> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="TodosController"/>.
    /// </summary>
    public TodosBatchController(IMediator mediator, ILogger<TodosController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Batch create todos.
    /// </summary>
    [HttpPost("batch")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoDto>>), StatusCodes.Status201Created)]
    public async Task<IActionResult> BatchCreate([FromBody] IEnumerable<CreateTodoRequest> requests, CancellationToken ct)
    {
        var cmd = new BatchCreateTodosCommand(requests.Select(r => (r.Title, r.Description)).ToArray());
        var result = await _mediator.Send(cmd, ct);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// Batch mark as done.
    /// </summary>
    [HttpPatch("batch/done")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BatchMarkDone([FromBody] BatchMarkDoneRequest request, CancellationToken ct)
    {
        var cmd = new BatchMarkTodosDoneCommand(request.Ids, request.Done);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    /// <summary>
    /// Batch delete.
    /// </summary>
    [HttpDelete("batch")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<BatchMarkTodosDoneCommandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BatchDelete([FromBody] BatchDeleteRequest request, CancellationToken ct)
    {
        var cmd = new BatchDeleteTodosCommand(request.Ids);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }
}
