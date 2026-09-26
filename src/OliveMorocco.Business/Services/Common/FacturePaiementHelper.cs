using OliveMorocco.Business.DTOs.Common;

namespace OliveMorocco.Business.Services.Common;

public static class FacturePaiementHelper
{
    public static IReadOnlyList<CreateFacturePaiementDto> Normalize(
        IEnumerable<CreateFacturePaiementDto>? paiements) =>
        (paiements ?? [])
            .Where(p => p.Montant > 0)
            .ToList();

    public static bool ComputeEstPayee(decimal totalTtc, IEnumerable<CreateFacturePaiementDto> paiements)
    {
        if (totalTtc <= 0)
            return false;

        var encaisse = paiements.Where(p => p.EstEncaisse).Sum(p => p.Montant);
        return encaisse >= totalTtc;
    }

    public static decimal SumEncaisse(IEnumerable<CreateFacturePaiementDto> paiements) =>
        paiements.Where(p => p.EstEncaisse).Sum(p => p.Montant);

    public static decimal SumEnAttente(IEnumerable<CreateFacturePaiementDto> paiements) =>
        paiements.Where(p => !p.EstEncaisse).Sum(p => p.Montant);
}
