using OliveMorocco.Business.DTOs.Common;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Common;

namespace OliveMorocco.Business.Services.Vente;

internal static class FactureClientDtoPreparer
{
    public static CreateFactureClientDto PrepareCreate(CreateFactureClientDto dto, string numero)
    {
        var lignes = SanitizeLines(dto.Lignes);
        var paiements = FacturePaiementHelper.Normalize(dto.Paiements);
        var (_, _, ttc) = IFactureClientService.ComputeTotals(lignes, dto.RemiseGlobale);

        return dto with
        {
            Numero = numero,
            Note = TrimOrEmpty(dto.Note),
            BonCommandeReference = TrimOrEmpty(dto.BonCommandeReference),
            Lignes = lignes,
            Paiements = paiements,
            TotalTtc = ttc,
            EstPayee = FacturePaiementHelper.ComputeEstPayee(ttc, paiements),
        };
    }

    public static (UpdateFactureClientDto Dto, bool EstPayee) PrepareUpdate(UpdateFactureClientDto dto)
    {
        var lignes = SanitizeLines(dto.Lignes);
        var paiements = FacturePaiementHelper.Normalize(dto.Paiements);
        var (_, _, ttc) = IFactureClientService.ComputeTotals(lignes, dto.RemiseGlobale);

        var prepared = dto with
        {
            Note = TrimOrEmpty(dto.Note),
            BonCommandeReference = TrimOrEmpty(dto.BonCommandeReference),
            Lignes = lignes,
            Paiements = paiements,
            TotalTtc = ttc,
        };

        return (prepared, FacturePaiementHelper.ComputeEstPayee(ttc, paiements));
    }

    private static IReadOnlyList<CreateFactureClientLigneDto> SanitizeLines(
        IReadOnlyList<CreateFactureClientLigneDto> lignes) =>
        lignes
            .Select(l => l with { Conditionnement = TrimOrEmpty(l.Conditionnement) })
            .ToList();

    private static string TrimOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}
