using OliveMorocco.Web.Models.Shared;

namespace OliveMorocco.Web.Models.Achat.Fournisseurs;

/// <summary>
/// Arabic for the Suppliers document.
///
/// A supplier is the mirror of a client, and its form is deliberately the same seven fields
/// in the same order — that symmetry is why <c>Nom</c>, <c>ICE</c>, <c>Adresse</c>,
/// <c>Ville</c>, <c>Téléphone</c>, <c>E-mail</c> and <c>Conditions de paiement</c> resolve
/// identically from here and from <c>Vente\Clients\ClientTranslations</c>. They are listed
/// in both files on purpose: someone translating one document should not have to open the
/// other to discover what "ICE" means, and if one of them ever needs different wording the
/// merge order in <see cref="Translations"/> makes that a deliberate choice rather than an
/// accident.
/// </summary>
public static class FournisseurTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Fournisseurs"] = "الموردون",
            ["fournisseur"] = "مورد",
            ["Nouveau fournisseur"] = "مورد جديد",
            ["Modifier fournisseur"] = "تعديل المورد",
            ["Huileries, emballages, intrants"] = "معاصر وتغليف ومدخلات",

            // --- The seven fields, identical to the client form ---
            ["Nom"] = "الاسم",
            ["ICE"] = "التعريف الموحّد",
            ["Adresse"] = "العنوان",
            ["Ville"] = "المدينة",
            ["Téléphone"] = "الهاتف",
            ["E-mail"] = "البريد الإلكتروني",
            ["Conditions de paiement"] = "شروط الدفع",

            ["Supprimer ce fournisseur ?"] = "حذف هذا المورد؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Achat · Fournisseurs"] = "الشراء · الموردون",
        };
}
