using backend.Model;

namespace backend.Repository;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync();
}
