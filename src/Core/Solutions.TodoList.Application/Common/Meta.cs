namespace Solutions.TodoList.Application.Common;

/// <summary>
/// Minimal meta information (pagination / trace).
/// </summary>
public sealed record Meta(int Skip = 1, int Take = 20, long TotalCount = 0, string? TraceId = null);