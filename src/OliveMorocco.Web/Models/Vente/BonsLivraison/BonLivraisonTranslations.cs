namespace OliveMorocco.Web.Models.Vente.BonsLivraison;

/// <summary>
/// Arabic for the delivery note.
///
/// A delivery note is an order being fulfilled, so it carries two quantities where every
/// other sales document has one: what was ordered and what actually arrived. That pair is
/// this document's reason for existing in the vocabulary, and it is why its line table is
/// two columns wider than the others even though the rest is shared.
/// </summary>
public static class BonLivraisonTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {

            ["bon de livraison"] = "أمر التسليم",
            ["Nouveau bon de livraison"] = "أمر تسليم جديد",
            ["Modifier bon de livraison"] = "تعديل أمر التسليم",
            ["Livraisons clients"] = "تسليمات الزبائن",

            // --- The two quantities that make this document different ---
            ["Qté cmd."] = "الكمية المطلوبة",
            ["Qté livrée"] = "الكمية المسلَّمة",

            ["Client"] = "الزبون",

            ["Supprimer ce bon de livraison ?"] = "حذف أمر التسليم هذا؟",
        };
}
