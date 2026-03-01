using backend.Dtos;
using backend.Repository;

namespace backend.Controllers;

public static class NoteEndpoints
{
    public static void MapNoteControllers(this WebApplication app)
    {
        var notes = app.MapGroup("/api/v1/notes");

        notes.MapGet("/", async (string? title, int? type, INoteRepository repo) =>
        {
            var data = await repo.GetAllAsync(title, type);
            return Results.Ok(data);
        });

        notes.MapGet("/{id}", async (int id, INoteRepository repo) =>
        {
            var note = await repo.GetByIdAsync(id);
            return note is not null ? Results.Ok(note) : Results.NotFound();
        });

        notes.MapPost("/", async (CreateNoteRequest request, INoteRepository repo) =>
        {
            var created = await repo.CreateAsync(request);
            return Results.Created($"/api/v1/notes/{created.Id}", created);
        });

        notes.MapPut("/{id}", async (int id, UpdateNoteRequest request, INoteRepository repo) =>
        {
            var updated = await repo.UpdateAsync(id, request);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        });

        notes.MapDelete("/{id}", async (int id, INoteRepository repo) =>
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}
