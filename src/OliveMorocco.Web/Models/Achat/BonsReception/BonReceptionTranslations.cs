namespace OliveMorocco.Web.Models.Achat.BonsReception;

/// <summary>
/// Arabic for the goods-received note.
///
/// The receiving end of a supplier order, and the point at which input stock actually
/// increases — which is why it has its own quantity column ("received") rather than the
/// ordered one. A reception can also point forward to the invoice it was billed on, so it
/// is the only purchase document with a link out to a second document.
/// </summary>
public static class BonReceptionTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {

            ["bon de réception"] = "أمر الاستلام",
            ["Nouveau bon de réception"] = "أمر استلام جديد",
            ["Modifier bon de réception"] = "تعديل أمر الاستلام",
            ["Livraisons fournisseurs"] = "تسليمات الموردين",

            // --- The quantity that lands in stock ---
            ["Qté reçue"] = "الكمية المستلَمة",

            // --- Forward link to the invoice ---

            ["Voir facture"] = "عرض الفاتورة",

            ["Fournisseur"] = "المورد",

            ["Supprimer ce bon de réception ?"] = "حذف أمر الاستلام هذا؟",
        };
}
