using Dapper;
using Microsoft.Data.SqlClient;
using backend.Model;

namespace backend.Repository;

public sealed class NoteImplement : INoteRepository
{
    private readonly string _connectionString;

    public NoteImplement(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServer is missing.");
    }

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        await using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<Note>(
            """
            SELECT
                id AS Id,
                title AS Title,
                content AS Content,
                type AS Type,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM dbo.Notes
            ORDER BY id DESC;
            """);
    }
}
