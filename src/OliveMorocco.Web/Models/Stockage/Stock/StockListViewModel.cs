using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Stock;

public sealed class StockListViewModel
{
    public const int DefaultPageSize = 15;
    public const string VueProduits = "produits";
    public const string VueHuile = "huile";
    public const string VueIntrant = "intrant";

    /// <summary>Active tab: <see cref="VueProduits"/> (bottled products), <see cref="VueHuile"/> (bulk oil per variety), or <see cref="VueIntrant"/> (intrants).</summary>
    public string Vue { get; init; } = VueProduits;

    public bool IsHuile => Vue == VueHuile;

    public bool IsIntrant => Vue == VueIntrant;

    public IReadOnlyList<StockEtatListItemDto> Items { get; init; } = [];

    public IReadOnlyList<StockHuileListItemDto> HuileItems { get; init; } = [];

    public IReadOnlyList<IntrantStockListItemDto> IntrantItems { get; init; } = [];

    public string? Search { get; init; }

    public bool StockBasOnly { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
