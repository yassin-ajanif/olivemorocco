namespace OliveMorocco.Web.Models.Shared;

public sealed class ListPaginationViewModel
{
    public int Page { get; init; } = 1;

    public int TotalPages { get; init; } = 1;

    public bool HasPrevious { get; init; }

    public bool HasNext { get; init; }

    public string? Search { get; init; }

    /// <summary>Route id for detail pages (e.g. stock mouvements on /Stock/Detail/{id}).</summary>
    public int? Id { get; init; }

    /// <summary>Stock list filter — preserve « stock bas uniquement » across pages.</summary>
    public bool StockBasOnly { get; init; }
}
