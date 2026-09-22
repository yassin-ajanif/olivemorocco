namespace OliveMorocco.Business.DTOs;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
