namespace OliveMorocco.Web.Models.Vente.Devis;

/// <summary>
/// Arabic for the Quote document.
///
/// A quote is the only sales document with a validity date and the only one where the
/// client is a prospect rather than a debtor, which is what its one-line description says.
/// Its line table, totals and search boxes come from
/// <see cref="Shared.SalesDocumentTranslations"/> — those are the same on all nine
/// documents — so this file holds only what is specific to quoting.
/// </summary>
public static class DevisTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Devis"] = "عروض الأسعار",
            ["devis"] = "عرض سعر",
            ["Nouveau devis"] = "عرض سعر جديد",
            ["Modifier devis"] = "تعديل عرض السعر",
            ["Propositions commerciales clients"] = "عروض تجارية للزبائن",

            // --- A quote's own header fields ---
            ["Date du devis"] = "تاريخ عرض السعر",
            ["Valable jusqu'au"] = "صالح حتى",
            ["Validité"] = "الصلاحية",
            ["Client"] = "الزبون",

            // --- Its search boxes ---

            ["Supprimer ce devis ?"] = "حذف عرض السعر هذا؟",
        };
}
