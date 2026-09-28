using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Stock;

public sealed class StockListViewModel
{
    public const int DefaultPageSize = 15;
    public const string VueProduits = "produits";
    public const string VueHuile = "huile";

    /// <summary>Active tab: <see cref="VueProduits"/> (bottled products) or <see cref="VueHuile"/> (bulk oil per variety).</summary>
    public string Vue { get; init; } = VueProduits;

    public bool IsHuile => Vue == VueHuile;

    public IReadOnlyList<StockEtatListItemDto> Items { get; init; } = [];

    public IReadOnlyList<StockHuileListItemDto> HuileItems { get; init; } = [];

    public string? Search { get; init; }

    public bool StockBasOnly { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
