namespace Solutions.TodoList.Application.Common;

/// <summary>
/// Paged result used for list endpoints.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, long TotalCount);