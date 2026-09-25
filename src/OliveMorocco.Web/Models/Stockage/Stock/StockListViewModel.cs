using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Stock;

public sealed class StockListViewModel
{
    public const int DefaultPageSize = 15;

    public IReadOnlyList<StockEtatListItemDto> Items { get; init; } = [];

    public string? Search { get; init; }

    public bool StockBasOnly { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
