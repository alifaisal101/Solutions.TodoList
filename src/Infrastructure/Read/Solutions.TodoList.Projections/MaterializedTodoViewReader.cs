using System.Data;
using Dapper;
using Solutions.TodoList.Domain.Entities;

namespace Solutions.TodoList.Projections;

public class MaterializedTodoViewReader
{
    private readonly IDbConnection _connection;

    public MaterializedTodoViewReader(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Todo?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM materialized_todos WHERE id=@id";
        return await _connection.QuerySingleOrDefaultAsync<Todo>(sql, new { id });
    }

    public async Task<IReadOnlyList<Todo>> ListAsync(
        string? search, string? sort, int skip, int take, CancellationToken ct = default)
    {
        var sql = "SELECT * FROM materialized_todos WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(search))
            sql += " AND (title ILIKE @search OR description ILIKE @search)";
        sql += $" ORDER BY {(sort == "createdAt_desc" ? "createdat DESC" : "createdat ASC")}";
        sql += " OFFSET @skip LIMIT @take";
        return (await _connection.QueryAsync<Todo>(
            sql, new { search = $"%{search}%", skip, take })).AsList();
    }
}