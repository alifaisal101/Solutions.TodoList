using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Solutions.TodoList.Domain.Exceptions;

namespace Solutions.TodoList.WebApi.Errors;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = Map(exception);
        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError ? null : Unwrap(exception).Message
            }
        });
    }

    private static (int Status, string Title) Map(Exception exception) => Unwrap(exception) switch
    {
        DomainValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static Exception Unwrap(Exception exception) =>
        exception is JsonException { InnerException: { } inner } ? inner : exception;
}
