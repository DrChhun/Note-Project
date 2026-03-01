namespace backend.Controllers;

public static class NoteEndpoints
{
    private static readonly List<Note> Notes =
    [
        new Note(Guid.NewGuid(), "First Note", "Hello from the API.", DateTime.UtcNow),
    ];

    public static void MapNoteControllers(this WebApplication app)
    {
        var notes = app.MapGroup("/api/v1/notes");

        notes.MapGet("/", async Task<IResult> () =>
        {
            return Results.Ok(Notes);
        });
    }
}

public record Note(Guid Id, string Title, string Content, DateTime CreatedAt);
