namespace backend.Dtos;

public record UpdateNoteRequest(string Title, string? Content, int Type);
