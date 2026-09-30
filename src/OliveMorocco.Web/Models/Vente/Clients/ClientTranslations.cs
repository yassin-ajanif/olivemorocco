using OliveMorocco.Web.Models.Shared;

namespace OliveMorocco.Web.Models.Vente.Clients;

/// <summary>
/// Arabic for everything the Clients document says about itself.
///
/// One file per document, in the document's own folder, so translating Clients means
/// opening this file and nothing else — no shared pile to scan, no other module's words in
/// sight to be edited by accident. It covers the three views that make up the document:
/// Index (the list), Create and Edit (both through _ClientForm), plus the browser tab
/// titles DashboardNav.PageTitle builds for it.
///
/// Keys are the exact French strings the views print. If a label is reworded in the Razor,
/// the key here stops matching and the gloss silently disappears rather than going stale
/// against wording that no longer exists — see <see cref="TranslationsAttribute"/>.
///
/// Two deliberate exclusions:
///
/// <list type="bullet">
/// <item>Words the shell owns — "Enregistrer", "Actions", "Actif" — are in
/// <see cref="UiTranslations"/>, not repeated here.</item>
/// <item>"Vente · Clients", the breadcrumb on Create and Edit, has no key. It is a
/// composition of two words that each already have one, and gluing the Arabic together
/// would mean a separate key per page; the eyebrow stays French until the breadcrumb is
/// built from its parts instead of typed as one string.</item>
/// </list>
///
/// A word that means the same thing everywhere ("Nom") still belongs to this file rather
/// than to the shell, because it is a field of a client and not furniture. The next
/// document that claims the same word has to agree on the Arabic — see the note on
/// <see cref="TranslationsAttribute"/> before adding one.
/// </summary>
[Translations]
public static class ClientTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // --- Document name, as the sidebar and the list heading both print it ---
            ["Clients"] = "الزبائن",
            ["client"] = "زبون",
            ["Nouveau client"] = "زبون جديد",
            ["Modifier client"] = "تعديل زبون",
            ["Réf. client"] = "مرجع الزبون",
            ["Supprimer ce client ?"] = "حذف هذا الزبون؟",

            // --- Fields ---
            ["Nom"] = "الاسم",

            // ICE is "Identifiant Commun de l'Entreprise" in French and the tax identifier
            // every Moroccan invoice quotes. The Arabic is the spelled-out form rather than
            // the three Latin letters, which would be a gloss in the wrong script.
            ["ICE"] = "التعريف الموحّد",

            ["Adresse"] = "العنوان",
            ["Ville"] = "المدينة",
            ["Téléphone"] = "الهاتف",
            ["E-mail"] = "البريد الإلكتروني",
            ["Conditions de paiement"] = "شروط الدفع",

            // --- The line under the heading on the list, naming who the list is for ---
            ["Restaurateurs, importateurs, épiciers"] = "المطاعم والمستوردون والبقالة",
        };
}
