using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Web.Models.Shared;

public static class ModePaiementLabels
{
    public static string Label(ModePaiement mode) => mode switch
    {
        ModePaiement.Cheque => "Chèque",
        ModePaiement.Especes => "Espèces",
        ModePaiement.TPE => "TPE",
        ModePaiement.Virement => "Virement",
        ModePaiement.Effet => "Effet",
        _ => mode.ToString(),
    };
}
