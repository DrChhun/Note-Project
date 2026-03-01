namespace backend.Dtos;

public record CreateNoteRequest(string Title, string? Content, int Type = 0);
