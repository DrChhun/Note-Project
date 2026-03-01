using backend.Dtos;
using backend.Model;

namespace backend.Repository;

public interface INoteRepository
{
    Task<PagedResult<Note>> GetPagedAsync(string? title = null, int? type = null, int page = 1, int pageSize = 15);
    Task<Note?> GetByIdAsync(int id);
    Task<Note> CreateAsync(CreateNoteRequest request);
    Task<Note?> UpdateAsync(int id, UpdateNoteRequest request);
    Task<bool> DeleteAsync(int id);
}
