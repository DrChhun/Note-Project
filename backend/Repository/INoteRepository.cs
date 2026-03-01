using backend.Dtos;
using backend.Model;

namespace backend.Repository;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(string? title = null, int? type = null);
    Task<Note?> GetByIdAsync(int id);
    Task<Note> CreateAsync(CreateNoteRequest request);
    Task<Note?> UpdateAsync(int id, UpdateNoteRequest request);
    Task<bool> DeleteAsync(int id);
}
