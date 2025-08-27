namespace Solutions.TodoList.Application.Common;

/// <summary>
/// Standard API envelope used by controllers and handlers.
/// </summary>
/// <typeparam name="T">Payload type.</typeparam>
public record ApiResponse<T>(T? Data = default, Meta? Meta = null, bool Success = true, string? Message = null);