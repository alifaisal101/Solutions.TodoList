using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solutions.TodoList.Application.Common;
using Solutions.TodoList.Application.Features.Todo.Commands.CreateTodo;
using Solutions.TodoList.Application.Features.Todo.Commands.DeleteTodo;
using Solutions.TodoList.Application.Features.Todo.Commands.MarkTodoDone;
using Solutions.TodoList.Application.Features.Todo.Commands.UpdateTodo;
using Solutions.TodoList.Application.Features.Todo.Queries.GetTodoById;
using Solutions.TodoList.Application.Features.Todo.Queries.ListTodos;
using Solutions.TodoList.Application.Requests.Todo;
using Solutions.TodoList.Domain.Dtos;

namespace Solutions.TodoList.WebApi.Controllers;

/// <summary>
/// Controller for managing todos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TodosController> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="TodosController"/>.
    /// </summary>
    public TodosController(IMediator mediator, ILogger<TodosController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new todo.
    /// </summary>
    /// <param name="request">Create request.</param>
    /// <returns>Created todo DTO.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody][Required] CreateTodoRequest request, CancellationToken ct)
    {
        var cmd = new CreateTodoCommand(request.Title, request.Description);
        var result = await _mediator.Send(cmd, ct);

        // result should contain created TodoDto and the resource id
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Get todo by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var query = new GetTodoByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result.Data == null)
            return NotFound(new ProblemDetails { Title = "Resource Not Found", Status = StatusCodes.Status404NotFound });

        return Ok(result);
    }

    /// <summary>
    /// List todos with pagination / filtering / sorting.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] string? sort = "createdAt_desc",
        [FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
    {
        var query = new ListTodosQuery(search, sort, skip, take);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Full replace update for a todo.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTodoRequest request, CancellationToken ct)
    {
        var cmd = new UpdateTodoCommand(id, request.Title, request.Description);
        var result = await _mediator.Send(cmd, ct);

        if (result.Data == null)
            return NotFound(new ProblemDetails { Title = "Resource Not Found", Status = StatusCodes.Status404NotFound });

        return Ok(result);
    }

    /// <summary>
    /// Toggle/mark done endpoint (PATCH).
    /// </summary>
    [HttpPatch("{id:guid}/done")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkDone([FromRoute] Guid id, [FromBody] MarkDoneRequest request, CancellationToken ct)
    {
        var cmd = new MarkTodoDoneCommand(id, request.Done);
        var result = await _mediator.Send(cmd, ct);

        if (result.Data == null)
            return NotFound(new ProblemDetails { Title = "Resource Not Found", Status = StatusCodes.Status404NotFound });

        return Ok(result);
    }

    /// <summary>
    /// Hard delete a todo.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")] // example: only admin can delete (adjust per requirement)
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var cmd = new DeleteTodoCommand(id);
        var result = await _mediator.Send(cmd, ct);

        if (!result.Success)
            return NotFound(new ProblemDetails { Title = "Resource Not Found", Status = StatusCodes.Status404NotFound });

        return NoContent();
    }
}
