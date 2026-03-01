namespace backend.Dtos;

public record PagedResult<T>(IReadOnlyList<T> Payload, int Page, int PageSize, int TotalCount, int TotalPages);
