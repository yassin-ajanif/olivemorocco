namespace OliveMorocco.Web.Models.Operationnel.Pressages;

/// <summary>
/// Arabic for the Pressings document: olives in, oil out, with the yield that links them.
///
/// Yield is the whole point of this document and it appears twice — once per row in the
/// list, once on the form — so it is keyed under both wordings. The form's hint that yield
/// can be left blank to be calculated is worth translating properly, because leaving it in
/// French makes the field look mandatory when it is not.
/// </summary>
public static class PressageTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Pressages"] = "عمليات العصر",
            ["pressage"] = "عصر",
            ["Nouveau pressage"] = "عملية عصر جديدة",
            ["Modifier pressage"] = "تعديل عملية العصر",
            ["Pressage des olives à l'huilerie — rendement et facturation"] =
                "عصر الزيتونات في المعصر — المردودية والفوترة",

            // --- Where and what ---
            ["Huilerie"] = "المعصر",
            ["Variété"] = "الصنف",

            // --- The conversion itself ---
            ["Olives (kg)"] = "الزيتون (كغ)",
            ["Rendement"] = "المردودية",
            ["Rendement (%)"] = "المردودية (%)",
            ["Huile (L)"] = "الزيت (لتر)",
            ["Quantité d'olives (kg)"] = "كمية الزيتون (كغ)",
            ["Quantité d'huile (L)"] = "كمية الزيت (لتر)",
            ["Laisser vide pour calculer automatiquement (olives × rendement)."] =
                "اتركه فارغًا ليُحسب تلقائيًا (الزيتون × المردودية).",

            // --- The charge it came from ---
            ["Charge associée"] = "المصروف المرتبط",
            ["voir la charge"] = "عرض المصروف",
            ["Type de charge"] = "نوع المصروف",
            ["Date de la charge"] = "تاريخ المصروف",
            ["Type"] = "النوع",
            ["Libellé"] = "البيان",

            ["Note"] = "ملاحظة",

            // --- Lists ---
            ["Rechercher par huilerie, variété ou facture…"] =
                "البحث بالمعصر أو الصنف أو الفاتورة…",

            ["Supprimer ce pressage ?"] = "حذف عملية العصر هذه؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Opérationnel · Pressages"] = "العمليات · عمليات العصر",
        };
}
