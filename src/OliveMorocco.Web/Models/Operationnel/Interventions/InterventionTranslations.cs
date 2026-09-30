namespace OliveMorocco.Web.Models.Operationnel.Interventions;

/// <summary>
/// Arabic for the field-interventions document: one visit to one sector, what was used,
/// how much water was drawn, and what it cost.
///
/// This is the most self-contained form in the dashboard — it has no counterparties and no
/// line table, so almost everything it says is its own. Water is the part worth singling
/// out: the form lets you enter a flow rate and a duration and derives the volume, and the
/// hint explaining that is long enough to need its own Arabic.
/// </summary>
public static class InterventionTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Interventions"] = "التدخلات",
            ["intervention"] = "تدخل",
            ["Nouvelle intervention"] = "تدخل جديد",
            ["Modifier intervention"] = "تعديل التدخل",
            ["Opérations terrain par secteur (intrants, irrigation, coûts)"] =
                "عمليات ميدانية حسب القطاع (مدخلات، ري، تكاليف)",

            // --- Header ---
            ["Secteur"] = "القطاع",

            // --- What was used ---
            ["Intrants utilisés"] = "المدخلات المستعملة",
            ["Ajouter une ligne"] = "إضافة سطر",
            ["Intrant"] = "المدخل",
            ["Quantité"] = "الكمية",

            // --- Water ---
            ["Irrigation"] = "الري",
            ["Débit (m³/h)"] = "معدل التدفق (م³/س)",
            ["Durée (h)"] = "المدة (س)",
            ["Eau consommée (m³)"] = "الماء المستهلك (م³)",
            ["Eau (m³)"] = "الماء (م³)",
            ["Débit × durée remplit l'eau consommée ; vous pouvez aussi saisir la valeur manuellement."] =
                "معدل التدفق × المدة يملأ الماء المستهلك؛ يمكنك أيضًا إدخال القيمة يدويًا.",

            // --- What it cost ---
            ["Coûts"] = "التكاليف",
            ["Charges liées"] = "المصاريف المرتبطة",
            ["Ajouter une charge"] = "إضافة مصروف",
            ["Type"] = "النوع",
            ["Libellé"] = "البيان",

            ["Supprimer la charge"] = "حذف المصروف",

            // --- Lists ---
            ["Rechercher par secteur, intrant ou note…"] = "البحث بالقطاع أو المدخل أو الملاحظة…",

            ["Supprimer cette intervention ?"] = "حذف هذا التدخل؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Opérationnel · Interventions"] = "العمليات · التدخلات",
        };
}
