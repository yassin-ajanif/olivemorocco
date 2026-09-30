using System.Reflection;

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
/// This class holds no words of its own. They live in per-document files, one per module
/// that shows text, each marked with <see cref="TranslationsAttribute"/> — and the merge
/// below is what puts them together. Merging is lazy, so a request that renders no labels
/// pays nothing for it.
/// </para>
/// </summary>
public static class Translations
{
    /// <summary>The Arabic for a French label, or <c>null</c> when it has no translation yet.</summary>
    public static string? For(string? fr) =>
        fr is null ? null : Map.Value.GetValueOrDefault(fr);

    private static readonly Lazy<IReadOnlyDictionary<string, string>> Map = new(Merge);

    private static IReadOnlyDictionary<string, string> Merge()
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        // Seeded first so a document's own file always wins over this pile — see Pending.
        foreach (var (fr, ar) in Pending)
        {
            map[fr] = ar;
        }

        // Sorted by full type name because reflection order is not part of any contract.
        // Left unsorted, "the last module file wins" would depend on the order the runtime
        // happened to lay the metadata out in, and two people on two machines could get
        // different Arabic for the same word from the same source tree.
        foreach (var arabic in Tables().Select(Read))
        {
            foreach (var (fr, ar) in arabic)
            {
                map[fr] = ar;
            }
        }

        return map;
    }

    /// <summary>
    /// The translation tables declared with <see cref="TranslationsAttribute"/>, in a stable
    /// order.
    ///
    /// Only static classes count: a table is a bag of constants, and
    /// <see cref="TranslationsAttribute"/> cannot be put on anything else in practice, but the
    /// check is here so a future non-static table fails the same way everywhere instead of
    /// being instantiated by <see cref="Read"/> and quietly ignored.
    /// </summary>
    private static IEnumerable<Type> Tables() =>
        typeof(Translations).Assembly
            .GetTypes()
            .Where(t => t.IsDefined(typeof(TranslationsAttribute), inherit: false))
            .OrderBy(t => t.FullName, StringComparer.Ordinal);

    /// <summary>
    /// A table's <c>Arabic</c> property, read statically.
    ///
    /// By name rather than against a shared type because C# will not let a static class
    /// implement an interface (CS0714), so there is no type to cast to — see
    /// <see cref="TranslationsAttribute"/> for why that is the right trade here rather than
    /// making every table non-static to satisfy one.
    /// </summary>
    private static IReadOnlyDictionary<string, string> Read(Type table) =>
        (IReadOnlyDictionary<string, string>)table
            .GetProperty("Arabic", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    /// <summary>
    /// Words for the modules that have not been given their own file yet.
    ///
    /// Everything in the sidebar is here, so nothing was lost by moving Clients out to
    /// <see cref="Vente.Clients.ClientTranslations"/> as the first document to be split off.
    /// This dictionary is scaffolding, not the design: it is the thing each new document
    /// file is carved out of, and it should shrink to nothing as the last module lands.
    ///
    /// It is seeded before the module files in <see cref="Merge"/>, so a stale entry here
    /// can never quietly override the file that now owns the word.
    ///
    /// Note the singular entries at the end. <c>DashboardNav.PageTitle</c> builds
    /// "Nouveau {SingularLabel}" and "Modifier {SingularLabel}" from them, so the document
    /// titles are ready for the pass that wraps those headings.
    /// </summary>
    private static readonly Dictionary<string, string> Pending = new(StringComparer.Ordinal)
    {
        // --- Stockage ---
        ["Produits"] = "المنتجات",
        ["Variétés"] = "الأصناف",
        ["Intrants"] = "المدخلات",
        ["Secteurs"] = "القطاعات",
        ["État du stock"] = "حالة المخزون",

        // --- Vente ---
        ["Devis"] = "عروض الأسعار",
        ["Bons de commande"] = "أوامر الطلب",
        ["Bons de livraison"] = "أوامر التسليم",
        ["Facturation"] = "الفوترة",
        ["Avoirs"] = "الإرجاعات",

        // --- Achat ---
        ["Fournisseurs"] = "الموردون",
        ["Bons de réception"] = "أوامر الاستلام",
        ["Factures fournisseur"] = "فواتير الموردين",
        ["Avoir fournisseur"] = "إرجاع المورد",
        ["Charges"] = "المصاريف",

        // --- Opérationnel ---
        ["Interventions"] = "التدخلات",
        ["Pressages"] = "عمليات العصر",
        ["Remplissages"] = "عمليات التعبئة",

        // --- Singular document types, for the "Nouveau …" / "Modifier …" page titles ---
        ["devis"] = "عرض سعر",
        ["bon de commande"] = "أمر الطلب",
        ["bon de livraison"] = "أمر التسليم",
        ["facture"] = "فاتورة",
        ["avoir"] = "إرجاع",
        ["bon de réception"] = "أمر الاستلام",
        ["facture fournisseur"] = "فاتورة المورد",
        ["avoir fournisseur"] = "إرجاع المورد",
        ["produit"] = "منتج",
        ["intrant"] = "مدخل",
    };
}
