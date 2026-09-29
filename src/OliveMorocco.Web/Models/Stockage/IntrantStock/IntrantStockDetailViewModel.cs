using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.IntrantStock;

public sealed class IntrantStockDetailViewModel
{
    public const int DefaultMouvementPageSize = 15;

    public IntrantStockDetailDto Intrant { get; init; } = null!;

    public IReadOnlyList<MouvementIntrantListItemDto> Mouvements { get; init; } = [];

    public int MouvementPage { get; init; } = 1;

    public int MouvementTotalCount { get; init; }

    public int MouvementPageSize { get; init; } = DefaultMouvementPageSize;

    public int MouvementTotalPages =>
        Math.Max(1, (int)Math.Ceiling(MouvementTotalCount / (double)MouvementPageSize));

    public bool MouvementHasPrevious => MouvementPage > 1;

    public bool MouvementHasNext => MouvementPage < MouvementTotalPages;

    public IntrantStockAjustementViewModel Ajustement { get; init; } = new();
}
