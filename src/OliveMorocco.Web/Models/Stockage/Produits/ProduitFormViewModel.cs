using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Produits;

public sealed class ProduitFormViewModel
{
    public int? Id { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public int VarieteId { get; set; }

    public string Unite { get; set; } = string.Empty;

    public string? CodeBarre { get; set; }

    public decimal PrixAchatHT { get; set; }

    public decimal PrixVenteHT { get; set; }

    public decimal TauxTVA { get; set; } = 20;

    public decimal StockInitial { get; set; }

    public decimal? StockActuel { get; set; }

    public decimal StockMinimum { get; set; }

    public bool Actif { get; set; } = true;

    public IReadOnlyList<VarieteSelectItemDto> Varietes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
