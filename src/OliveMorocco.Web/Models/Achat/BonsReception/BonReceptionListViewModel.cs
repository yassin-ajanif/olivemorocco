using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Web.Models.Achat.BonsReception;

public sealed class BonReceptionListViewModel
{
    public const int DefaultPageSize = 15;

    public IReadOnlyList<BonReceptionListItemDto> Items { get; init; } = [];

    public string? Search { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
