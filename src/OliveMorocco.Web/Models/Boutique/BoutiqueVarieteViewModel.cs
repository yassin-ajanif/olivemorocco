namespace OliveMorocco.Web.Models.Boutique;

/// <summary>
/// A variety grouping for the shop. The grid is laid out one line per variety, each line
/// holding that variety's formats side by side, so the variety itself needs its own display
/// data (name, origin region, tasting notes) rather than just a name on the product.
/// </summary>
public sealed class BoutiqueVarieteViewModel
{
    public int Id { get; init; }

    public string Nom { get; init; } = string.Empty;

    public string? Code { get; init; }

    /// <summary>Shown in the hover badge — the region the variety is sourced from.</summary>
    public string? RegionOrigine { get; init; }

    /// <summary>Tasting notes, shown in the hover badge under the name and region.</summary>
    public string? Description { get; init; }

    /// <summary>The products of this variety, in display order (1 L, 5 L, 10 L).</summary>
    public IReadOnlyList<BoutiqueProduitViewModel> Produits { get; init; } = [];
}
