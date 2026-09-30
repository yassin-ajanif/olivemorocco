namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// Arabic for the dashboard labels, keyed by the French string the views already print.
///
/// One dictionary rather than an Arabic literal at each call site: there are dozens of
/// labels, and a translator editing wording should not have to hunt through Razor. The
/// sidebar, a page heading and a document title all say "Devis", so the keying also
/// guarantees they say the same thing in both languages.
///
/// Keys are case-sensitive and must match the markup exactly — <see cref="For"/> returns
/// <c>null</c> for anything it does not know, and the label then simply renders without a
/// gloss. Adding a label to a page before translating it is therefore harmless.
/// </summary>
public static class Translations
{
    /// <summary>The Arabic for a French label, or <c>null</c> when it has no translation yet.</summary>
    public static string? For(string? fr) =>
        fr is null ? null : Ar.GetValueOrDefault(fr);

    private static readonly Dictionary<string, string> Ar = new(StringComparer.Ordinal)
    {
        // --- sidebar sections ---
        ["Stockage"] = "التخزين",
        ["Vente"] = "البيع",
        ["Achat"] = "الشراء",
        ["Opérationnel"] = "العمليات",

        // --- Stockage ---
        ["Produits"] = "المنتجات",
        ["Variétés"] = "الأصناف",
        ["Intrants"] = "المدخلات",
        ["Secteurs"] = "القطاعات",
        ["État du stock"] = "حالة المخزون",

        // --- Vente ---
        ["Clients"] = "الزبائن",
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

        // --- Singular document types, for the "Nouveau …" / "Modifier …" page titles
        //     that DashboardNav.PageTitle builds from Module.SingularLabel. Listed here so
        //     the headings are ready before the next pass reaches them. ---
        ["devis"] = "عرض سعر",
        ["bon de commande"] = "أمر الطلب",
        ["bon de livraison"] = "أمر التسليم",
        ["facture"] = "فاتورة",
        ["client"] = "زبون",
        ["avoir"] = "إرجاع",
        ["bon de réception"] = "أمر الاستلام",
        ["facture fournisseur"] = "فاتورة المورد",
        ["avoir fournisseur"] = "إرجاع المورد",
        ["produit"] = "منتج",
        ["intrant"] = "مدخل",
    };
}