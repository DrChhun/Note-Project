using backend.Repository;

namespace backend.Controllers;

public static class NoteEndpoints
{
    public static void MapNoteControllers(this WebApplication app)
    {
        var notes = app.MapGroup("/api/v1/notes");

        notes.MapGet("/", async Task<IResult> (INoteRepository service) =>
        {
            var data = await service.GetAllAsync();
            return Results.Ok(data);
        });
    }
}