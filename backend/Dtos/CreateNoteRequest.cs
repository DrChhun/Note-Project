using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

public record CreateNoteRequest(
    [property: Required(ErrorMessage = "Title is required.")]
    [property: MinLength(1, ErrorMessage = "Title cannot be empty.")]
    [property: MaxLength(100, ErrorMessage = "Title must be at most 100 characters.")]
    string Title,
    [property: MaxLength(500, ErrorMessage = "Content must be at most 500 characters.")]
    string? Content,
    [property: Range(0, 3, ErrorMessage = "Type must be between 0 and 3.")]
    int Type = 0);
