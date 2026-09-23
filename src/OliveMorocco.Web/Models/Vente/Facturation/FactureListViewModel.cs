using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Web.Models.Vente.Facturation;

public sealed class FactureListViewModel
{
    public const int DefaultPageSize = 15;

    public IReadOnlyList<FactureClientListItemDto> Items { get; init; } = [];

    public string? Search { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
