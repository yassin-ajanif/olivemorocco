namespace OliveMorocco.Web.Models.Boutique;

/// <summary>
/// A product as the public shop renders it. Deliberately decoupled from the stock DTOs:
/// the shop shows a retail price, not purchase cost or the stock ledger, and it must not
/// grow the operational view models just to serve a marketing page.
/// </summary>
public sealed class BoutiqueProduitViewModel
{
    public int Id { get; init; }

    public string Reference { get; init; } = string.Empty;

    public string Designation { get; init; } = string.Empty;

    public int VarieteId { get; init; }

    public string VarieteNom { get; init; } = string.Empty;

    public string? VarieteCode { get; init; }

    /// <summary>Source region, shown in the variety hover badge.</summary>
    public string? VarieteRegion { get; init; }

    public string Unite { get; init; } = string.Empty;

    public decimal? ContenanceLitres { get; init; }

    /// <summary>URL of the product photo, or <c>null</c> to render the placeholder slot.</summary>
    public string? ImageUrl { get; init; }

    public decimal PrixVenteHT { get; init; }

    public decimal TauxTVA { get; init; }

    /// <summary>True when the product is orderable — the shop hides buy actions otherwise.</summary>
    public bool Disponible { get; init; }

    public decimal PrixVenteTTC => Math.Round(PrixVenteHT * (1 + TauxTVA / 100m), 2);

    /// <summary>Short capacity line for the card, e.g. "50 cl" or "1 L".</summary>
    public string ContenanceLabel => ContenanceLitres switch
    {
        null or 0 => string.Empty,
        < 1 => $"{ContenanceLitres.Value * 100:0.#} cl",
        _ => $"{ContenanceLitres.Value:0.##} L",
    };

    public string? CodeBarre { get; init; }
}
