using Dapper;
using Microsoft.Data.SqlClient;
using backend.Dtos;
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

    public async Task<IEnumerable<Note>> GetAllAsync(string? title = null, int? type = null)
    {
        await using var conn = new SqlConnection(_connectionString);
        var titlePattern = string.IsNullOrWhiteSpace(title) ? null : $"%{title.Trim()}%";
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
            WHERE (@TitlePattern IS NULL OR title LIKE @TitlePattern)
              AND (@Type IS NULL OR type = @Type)
            ORDER BY id DESC;
            """,
            new { TitlePattern = titlePattern, Type = type });
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Note>(
            """
            SELECT
                id AS Id,
                title AS Title,
                content AS Content,
                type AS Type,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM dbo.Notes
            WHERE id = @Id;
            """,
            new { Id = id });
    }

    public async Task<Note> CreateAsync(CreateNoteRequest request)
    {
        await using var conn = new SqlConnection(_connectionString);
        return await conn.QuerySingleAsync<Note>(
            """
            INSERT INTO dbo.Notes (title, content, type)
            OUTPUT
                INSERTED.id AS Id,
                INSERTED.title AS Title,
                INSERTED.content AS Content,
                INSERTED.type AS Type,
                INSERTED.created_at AS CreatedAt,
                INSERTED.updated_at AS UpdatedAt
            VALUES (@Title, @Content, @Type);
            """,
            new
            {
                request.Title,
                request.Content,
                request.Type
            });
    }

    public async Task<Note?> UpdateAsync(int id, UpdateNoteRequest request)
    {
        await using var conn = new SqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Note>(
            """
            UPDATE dbo.Notes
            SET
                title = @Title,
                content = @Content,
                type = @Type,
                updated_at = SYSDATETIME()
            OUTPUT
                INSERTED.id AS Id,
                INSERTED.title AS Title,
                INSERTED.content AS Content,
                INSERTED.type AS Type,
                INSERTED.created_at AS CreatedAt,
                INSERTED.updated_at AS UpdatedAt
            WHERE id = @Id;
            """,
            new
            {
                Id = id,
                request.Title,
                request.Content,
                request.Type
            });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        var rows = await conn.ExecuteAsync(
            """
            DELETE FROM dbo.Notes
            WHERE id = @Id;
            """,
            new { Id = id });

        return rows > 0;
    }
}
