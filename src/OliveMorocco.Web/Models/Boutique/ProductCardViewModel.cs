namespace OliveMorocco.Web.Models.Boutique;

/// <summary>
/// One card of the shared product layout, rendered by <c>Shared/_ProductCard.cshtml</c> and
/// used in three places: the boutique grid, the related-product strips on the detail page,
/// and the variety strip on the landing page.
/// <para>
/// One layout covers all three because the differences are expressed as nulls rather than
/// as a second component. The shop fills <see cref="PrixTTC"/> and gets a price block; the
/// landing page leaves it null and gets a taller photo instead. <see cref="Url"/> is passed
/// in rather than generated here because a static factory has no <c>IUrlHelper</c> — the
/// caller knows whether the card points at a detail page or at a filtered grid.
/// </para>
/// </summary>
public sealed class ProductCardViewModel
{
    /// <summary>Photo URL, or <c>null</c> to render the placeholder slot.</summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Gold pill over the photo. The landing page puts the region there; the shop leaves it
    /// null because its variety line already carries the name and region in the line head.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>Reference line above the title — a product ref, or a variety code.</summary>
    public string Ref { get; init; } = string.Empty;

    public string Titre { get; init; } = string.Empty;

    /// <summary>Line under the title. The landing page shows the tasting notes here.</summary>
    public string? Description { get; init; }

    /// <summary>Line under the title, when the card needs a qualifier the ref doesn't carry.</summary>
    public string? SousTitre { get; init; }

    /// <summary>Where the media, the title and the link all point.</summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>Link caption — "Voir le produit", or "Voir les 3 formats".</summary>
    public string LinkLabel { get; init; } = string.Empty;

    /// <summary>Price including tax, or <c>null</c> to render no price block at all.</summary>
    public decimal? PrixTTC { get; init; }

    public decimal TauxTVA { get; init; }

    /// <summary>Adds the scroll-reveal class. Only the landing page's cards are observed.</summary>
    public bool Reveal { get; init; }

    /// <summary>
    /// Stretches the photo to 3/4 instead of 4/5. Set by <see cref="PourVariete"/>: those
    /// cards carry no price, so the media absorbs the height it gives up.
    /// </summary>
    public bool Tall { get; init; }

    /// <summary>A product card: reference, designation, price, link to the detail page.</summary>
    public static ProductCardViewModel PourProduit(
        BoutiqueProduitViewModel produit,
        string url,
        string? sousTitre = null) => new()
        {
            ImageUrl = produit.ImageUrl,
            Ref = produit.Reference,
            Titre = produit.Designation,
            SousTitre = sousTitre,
            Url = url,
            LinkLabel = "Voir le produit",
            PrixTTC = produit.PrixVenteTTC,
            TauxTVA = produit.TauxTVA,
        };

    /// <summary>
    /// A variety card: the variety's own photo, its code, its name and its tasting notes,
    /// linking to that variety's line in the shop. No price — the landing page introduces
    /// the range, the shop is where figures get quoted.
    /// </summary>
    public static ProductCardViewModel PourVariete(BoutiqueVarieteViewModel variete, string url) => new()
        {
            ImageUrl = variete.PhotoVariete,
            Tag = variete.RegionOrigine,
            Ref = variete.Code ?? string.Empty,
            Titre = variete.Nom,
            Description = variete.Description,
            Url = url,
            LinkLabel = $"Voir les {variete.Produits.Count} formats",
            Reveal = true,
            Tall = true,
        };
}
