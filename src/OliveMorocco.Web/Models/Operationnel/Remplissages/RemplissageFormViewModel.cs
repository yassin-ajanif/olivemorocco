using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Web.Models.Operationnel.Remplissages;

public sealed class RemplissageFormViewModel
{
    public int? Id { get; set; }

    public string? Numero { get; set; }

    public int VarieteId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public decimal Perte { get; set; }

    public string? Note { get; set; }

    public IList<RemplissageLigneViewModel> Lignes { get; set; } = [];

    public IReadOnlyList<RemplissageVarieteSelectItemDto> Varietes { get; set; } = [];

    public IReadOnlyList<RemplissageProduitSelectItemDto> Produits { get; set; } = [];

    /// <summary>Liters already taken by this remplissage (edit) — added back to the available stock of its original variety.</summary>
    public decimal QuantiteHuileInitiale { get; set; }

    public int VarieteIdInitiale { get; set; }

    public bool IsEdit => Id.HasValue;
}

public sealed class RemplissageLigneViewModel
{
    public int ProduitId { get; set; }

    public decimal Quantite { get; set; }
}
