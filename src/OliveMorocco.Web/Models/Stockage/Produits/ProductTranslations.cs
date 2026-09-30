namespace OliveMorocco.Web.Models.Stockage.Produits;

/// <summary>
/// Arabic for the Products document: the catalogue of what is sold.
///
/// The sibling of <see cref="Varietes"/> — a product is a variety that has been bottled and
/// priced. The two share "Variété" and "Code", which is why those keys resolve identically
/// here and in VarieteTranslations: they name the same field of the same underlying thing,
/// so a reader must not see two words for it.
/// </summary>
public static class ProductTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Produits"] = "المنتجات",
            ["produit"] = "منتج",
            ["Nouveau produit"] = "منتج جديد",
            ["Modifier produit"] = "تعديل المنتج",
            ["Catalogue des produits vendus"] = "فهرس المنتجات المباعة",

            // --- Identity ---

            ["Code-barres"] = "الرمز الشريطي",

            // --- Classification ---
            ["Variété"] = "الصنف",
            ["Gérer les variétés"] = "إدارة الأصناف",

            // --- Selling ---

            ["Contenance (L)"] = "السعة (لتر)",
            ["Litres d'huile par unité — requis pour le remplissage."] =
                "لترات الزيت في الوحدة — مطلوبة لعملية التعبئة.",
            ["Taux TVA (%)"] = "نسبة الضريبة (%)",
            ["Prix achat HT (DH)"] = "سعر الشراء دون ضريبة (درهم)",
            ["Prix vente HT (DH)"] = "سعر البيع دون ضريبة (درهم)",
            ["P. vente HT"] = "سعر البيع دون ضريبة",

            // --- Stock ---
            ["Stock"] = "المخزون",
            ["Stock minimum"] = "المخزون الأدنى",
            ["Stock actuel"] = "المخزون الحالي",
            ["Stock min."] = "الحد الأدنى للمخزون",
            ["Stock bas"] = "مخزون منخفض",
            ["Prix achat HT"] = "سعر الشراء دون ضريبة",
            ["Rupture"] = "نفاد",

            // --- Status ---

            ["Stock initial"] = "المخزون الأولي",

            // --- Photo ---
            ["Photo du produit"] = "صورة المنتج",
            ["Aucune photo"] = "لا توجد صورة",
            ["Remplacer la photo"] = "استبدال الصورة",
            ["Supprimer la photo"] = "حذف الصورة",

            // --- Lists ---
            ["Rechercher par réf., désignation ou code-barres…"] =
                "البحث بالمرجع أو التسمية أو الرمز الشريطي…",
            ["Modifier depuis l'intervention"] = "تعديل من التدخل",
            ["Modifiable depuis l'intervention"] = "قابل للتعديل من التدخل",
            ["Modifier (depuis l'intervention)"] = "تعديل (من التدخل)",

            ["Supprimer ce produit ?"] = "حذف هذا المنتج؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Stockage · Produits"] = "التخزين · المنتجات",
        };
}
