using OliveMorocco.Web.Models.Achat.AvoirFournisseur;
using OliveMorocco.Web.Models.Achat.BonsCommande;
using OliveMorocco.Web.Models.Achat.BonsReception;
using OliveMorocco.Web.Models.Achat.Charges;
using OliveMorocco.Web.Models.Achat.FacturesFournisseurs;
using OliveMorocco.Web.Models.Achat.Fournisseurs;
using OliveMorocco.Web.Models.Operationnel.Interventions;
using OliveMorocco.Web.Models.Operationnel.Pressages;
using OliveMorocco.Web.Models.Operationnel.Remplissages;
using OliveMorocco.Web.Models.Stockage.Intrants;
using OliveMorocco.Web.Models.Stockage.Produits;
using OliveMorocco.Web.Models.Stockage.Secteurs;
using OliveMorocco.Web.Models.Stockage.Stock;
using OliveMorocco.Web.Models.Stockage.Varietes;
using OliveMorocco.Web.Models.Vente.Avoirs;
using OliveMorocco.Web.Models.Vente.BonsCommande;
using OliveMorocco.Web.Models.Vente.BonsLivraison;
using OliveMorocco.Web.Models.Vente.Clients;
using OliveMorocco.Web.Models.Vente.Devis;
using OliveMorocco.Web.Models.Vente.Facturation;

namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// The Arabic for a dashboard label: the one place a view asks.
///
/// <c>_TrLabel</c> hands every label it renders to <see cref="For"/>, keyed by the French
/// string the view already prints. That is what keeps the sidebar, a page heading and a
/// browser tab title saying the same thing — they all ask for "Devis" and they all get the
/// same word back, so there is no arrangement of pages in which two of them disagree.
///
/// <para>
/// This class holds no words of its own. They live in one file per document, in the
/// document's own folder, and <see cref="Tables"/> is the only place a new document has to
/// be mentioned. The list is a plain array of expressions rather than something discovered
/// at runtime on purpose: renaming a table's <c>Arabic</c> property then fails the build
/// instead of quietly compiling and leaving every gloss on that page missing. One line per
/// document is a fair price for that.
/// </para>
/// </summary>
public static class Translations
{
    /// <summary>The Arabic for a French label, or <c>null</c> when it has no translation yet.</summary>
    public static string? For(string? fr) =>
        fr is null ? null : Map.Value.GetValueOrDefault(fr);

    /// <summary>
    /// The Arabic for a label that lives inside an attribute — a placeholder, a tooltip, a
    /// screen reader's label — falling back to the French when there is no Arabic, so the
    /// attribute always has a value.
    ///
    /// <para>
    /// <see cref="_TrLabel"/> renders element content, and a partial can only render
    /// content: there is no way for it to put a word into an attribute. A tag helper would
    /// normally be the answer, and it is the wrong one here. The dashboard's forms write
    /// conditional attributes as bare <c>@(Model.X ? "disabled" : null)</c> — a disabled
    /// fieldset, a read-only input — and Razor only tolerates that inside an element's
    /// attribute list when no tag helper is bound to it. Binding one to every element to
    /// reach a single attribute turns each of those into RZ1031, a build error on every
    /// form in the application. A helper called by name writes the same attribute with none
    /// of that cost.
    /// </para>
    ///
    /// <para>
    /// The fallback is what makes the attribute safe to write without checking first: a
    /// label with no Arabic prints its own French rather than printing nothing. The
    /// companion <c>data-tr-ar</c> the views carry therefore means "the value to show while
    /// the switch is on" and nothing more — for an untranslated word that is simply the
    /// French, and switching it changes nothing.
    /// </para>
    /// </summary>
    public static string Ar(string fr) => Map.Value.GetValueOrDefault(fr) ?? fr;

    /// <summary>
    /// Every translation table, in the order they are merged.
    ///
    /// <see cref="UiTranslations"/> comes first so a document's own wording wins where the
    /// two overlap: "Actif" means the same thing everywhere today, but "Conditions de
    /// paiement" belongs to the client and the supplier, and if either of them ever wanted
    /// different words the document must not have to fight the shell for them.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string>[] Tables =
    [
        UiTranslations.Arabic,
        SalesDocumentTranslations.Arabic,
        ProductTranslations.Arabic,
        VarieteTranslations.Arabic,
        IntrantTranslations.Arabic,
        SecteurTranslations.Arabic,
        StockTranslations.Arabic,
        ClientTranslations.Arabic,
        DevisTranslations.Arabic,
        BonCommandeVenteTranslations.Arabic,
        BonLivraisonTranslations.Arabic,
        FacturationTranslations.Arabic,
        AvoirVenteTranslations.Arabic,
        FournisseurTranslations.Arabic,
        BonCommandeAchatTranslations.Arabic,
        BonReceptionTranslations.Arabic,
        FactureFournisseurTranslations.Arabic,
        AvoirFournisseurTranslations.Arabic,
        ChargeTranslations.Arabic,
        InterventionTranslations.Arabic,
        PressageTranslations.Arabic,
        RemplissageTranslations.Arabic,
    ];

    private static readonly Lazy<IReadOnlyDictionary<string, string>> Map = new(Merge);

    private static IReadOnlyDictionary<string, string> Merge()
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var table in Tables)
        {
            foreach (var (fr, ar) in table)
            {
                map[fr] = ar;
            }
        }

        return map;
    }
}
