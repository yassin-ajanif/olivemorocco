namespace OliveMorocco.Web.Models.Operationnel.Remplissages;

/// <summary>
/// Arabic for the Bottling document: bulk oil of one variety going into products.
///
/// The only document where the reader is watching liquid disappear, so it carries the two
/// figures that follow from that — litres lost, and what is left in bulk — plus the warning
/// that the stock is too small. Those are its own; the rest is ordinary list furniture.
/// </summary>
public static class RemplissageTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Remplissages"] = "عمليات التعبئة",
            ["remplissage"] = "تعبئة",
            ["Nouveau remplissage"] = "عملية تعبئة جديدة",
            ["Modifier remplissage"] = "تعديل عملية التعبئة",
            ["Mise en bouteille — l'huile en vrac d'une variété passe dans le stock des produits"] =
                "التعبئة في الزجاجات — الزيت بالجملة لصنف واحد ينتقل إلى مخزون المنتجات",

            // --- What is being bottled ---
            ["Variété"] = "الصنف",

            ["Numéro"] = "رقم",

            ["Produits"] = "المنتجات",
            ["Produits remplis"] = "المنتجات المعبأة",
            ["Produit"] = "المنتج",
            ["Ajouter une ligne"] = "إضافة سطر",
            ["Unités"] = "الوحدات",
            ["Contenance"] = "السعة",
            ["Litres"] = "لترات",

            // --- Oil lost, and oil accounted for ---
            ["Perte (L)"] = "الفقد (لتر)",
            ["Huile perdue pendant le remplissage (fond de cuve, débordement)."] =
                "الزيت الضائع أثناء التعبئة (قاع الخزان، الفيضان).",
            ["Huile disponible"] = "الزيت المتوفر",
            ["Litres en bouteilles"] = "لترات في الزجاجات",
            ["Huile utilisée (avec perte)"] = "الزيت المستعمل (مع الفقد)",
            ["Reste en vrac"] = "المتبقي بالجملة",
            ["Stock d'huile insuffisant pour cette variété."] =
                "مخزون الزيت غير كافٍ لهذا الصنف.",
            ["Seuls les produits actifs de la variété choisie, avec une contenance (L) renseignée dans la fiche produit, sont proposés."] =
                "يُقترح فقط المنتجات النشطة للصنف المختار، والمذكورة لها سعة (لتر) في بطاقة المنتج.",
            ["Note"] = "ملاحظة",

            // --- Lists ---
            ["Rechercher par numéro, variété ou produit…"] =
                "البحث بالرقم أو الصنف أو المنتج…",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Opérationnel · Remplissages"] = "العمليات · عمليات التعبئة",
        };
}
