namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// The dashboard's own words — the chrome every page is built from, regardless of which
/// document it is showing.
///
/// <para>
/// These live here rather than in each document's file for one reason: they are not fields
/// of anything. "Enregistrer", "Actions", "Page suivante" and "Vente" are printed by every
/// list and every form in the section. If each document's file carried its own copy, six
/// files would each hold a version of the same eleven buttons and the day one was reworded
/// the other five would still say the old thing — which is exactly the drift a translation
/// file is supposed to prevent.
///
/// A document's own field labels do belong to that document, even when the same word also
/// appears elsewhere: Vente.Clients.ClientTranslations keeps its "Nom" and "Ville" because
/// those are fields of a client, not furniture.
/// </para>
/// </summary>
[Translations]
public static class UiTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // --- Section names: the sidebar group headers and the breadcrumb above each page ---
            ["Stockage"] = "التخزين",
            ["Vente"] = "البيع",
            ["Achat"] = "الشراء",
            ["Opérationnel"] = "العمليات",

            // --- Shell ---
            ["Accueil"] = "الرئيسية",
            ["Ouvrir le menu"] = "فتح القائمة",
            ["Fermer le menu"] = "إغلاق القائمة",

            // --- The translation switch's own two captions ---
            ["Traduction"] = "ترجمة",
            ["Masquer"] = "إخفاء",

            // --- List toolbar and empty state ---
            ["Nouveau"] = "جديد",
            ["Rechercher…"] = "بحث…",
            ["Aucun enregistrement — cliquez sur Nouveau pour commencer."] =
                "لا توجد سجلات — انقر على «جديد» للبدء.",

            // --- Row actions and form buttons ---
            ["Actions"] = "إجراءات",
            ["Modifier"] = "تعديل",
            ["Enregistrer"] = "حفظ",
            ["Annuler"] = "إلغاء",
            ["Supprimer"] = "حذف",

            // --- The active/inactive toggle every "master data" list carries ---
            ["Actif"] = "نشط",
            ["Inactif"] = "غير نشط",
            ["Activer"] = "تفعيل",
            ["Désactiver"] = "تعطيل",
            ["On"] = "نعم",
            ["Off"] = "لا",

            // --- Pagination ---
            ["Première page"] = "الصفحة الأولى",
            ["Page précédente"] = "الصفحة السابقة",
            ["Page suivante"] = "الصفحة التالية",
            ["Dernière page"] = "الصفحة الأخيرة",
        };
}
