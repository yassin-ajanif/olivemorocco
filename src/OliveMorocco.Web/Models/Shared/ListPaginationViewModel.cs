namespace OliveMorocco.Web.Models.Shared;

public sealed class ListPaginationViewModel
{
    public int Page { get; init; } = 1;

    public int TotalPages { get; init; } = 1;

    public bool HasPrevious { get; init; }

    public bool HasNext { get; init; }

    public string? Search { get; init; }
}
