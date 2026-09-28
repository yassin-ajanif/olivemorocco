using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Web.Models.Stockage.Stock;

namespace OliveMorocco.Web.Models.Stockage.HuileVrac;

public sealed class HuileVracDetailViewModel
{
    public const int DefaultMouvementPageSize = 15;

    public StockHuileDetailDto Variete { get; init; } = null!;

    public IReadOnlyList<MouvementStockListItemDto> Mouvements { get; init; } = [];

    public int MouvementPage { get; init; } = 1;

    public int MouvementTotalCount { get; init; }

    public int MouvementPageSize { get; init; } = DefaultMouvementPageSize;

    public int MouvementTotalPages =>
        Math.Max(1, (int)Math.Ceiling(MouvementTotalCount / (double)MouvementPageSize));

    public bool MouvementHasPrevious => MouvementPage > 1;

    public bool MouvementHasNext => MouvementPage < MouvementTotalPages;

    public StockAjustementViewModel Ajustement { get; init; } = new();
}
