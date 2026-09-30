using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Produits;

public sealed class ProduitFormViewModel
{
    public int? Id { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public int VarieteId { get; set; }

    public string Unite { get; set; } = string.Empty;

    public decimal? ContenanceLitres { get; set; }

    public string? CodeBarre { get; set; }

    public decimal PrixAchatHT { get; set; }

    public decimal PrixVenteHT { get; set; }

    public decimal TauxTVA { get; set; } = 20;

    public decimal StockInitial { get; set; }

    public decimal StockMinimum { get; set; }

    public bool Actif { get; set; } = true;

    /// <summary>
    /// Photo currently on file for this product, rendered as a preview. Display only:
    /// the form has no URL box, so nothing posts this back, and the controller reads the
    /// real value from the database rather than trusting anything the client sent.
    /// </summary>
    public string? CurrentImageUrl { get; set; }

    /// <summary>
    /// Optional upload. When present it is written to the photo store and becomes the
    /// product's photo, replacing whatever was there.
    /// </summary>
    public IFormFile? ImageFile { get; set; }

    /// <summary>
    /// Marks the current photo for removal. Applied on save rather than on click, so a
    /// misclick followed by "Annuler" destroys nothing and the form stays all-or-nothing
    /// like every other field on it.
    /// </summary>
    public bool RemoveImage { get; set; }

    public IReadOnlyList<VarieteSelectItemDto> Varietes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
