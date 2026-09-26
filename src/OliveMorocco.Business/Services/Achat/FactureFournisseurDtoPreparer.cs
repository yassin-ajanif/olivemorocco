using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.DTOs.Common;
using OliveMorocco.Business.Services.Common;

namespace OliveMorocco.Business.Services.Achat;

internal static class FactureFournisseurDtoPreparer
{
    public static CreateFactureFournisseurDto PrepareCreate(CreateFactureFournisseurDto dto, string numero)
    {
        var lignes = SanitizeLines(dto.Lignes);
        var paiements = FacturePaiementHelper.Normalize(dto.Paiements);
        var (_, _, ttc) = IFactureFournisseurService.ComputeTotals(lignes, dto.RemiseGlobale);

        return dto with
        {
            Numero = numero,
            Note = TrimOrEmpty(dto.Note),
            Lignes = lignes,
            Paiements = paiements,
            TotalTtc = ttc,
            EstPayee = FacturePaiementHelper.ComputeEstPayee(ttc, paiements),
        };
    }

    public static (UpdateFactureFournisseurDto Dto, bool EstPayee) PrepareUpdate(UpdateFactureFournisseurDto dto)
    {
        var lignes = SanitizeLines(dto.Lignes);
        var paiements = FacturePaiementHelper.Normalize(dto.Paiements);
        var (_, _, ttc) = IFactureFournisseurService.ComputeTotals(lignes, dto.RemiseGlobale);

        var prepared = dto with
        {
            Note = TrimOrEmpty(dto.Note),
            Lignes = lignes,
            Paiements = paiements,
            TotalTtc = ttc,
        };

        return (prepared, FacturePaiementHelper.ComputeEstPayee(ttc, paiements));
    }

    private static IReadOnlyList<CreateFactureFournisseurLigneDto> SanitizeLines(
        IReadOnlyList<CreateFactureFournisseurLigneDto> lignes) =>
        lignes
            .Select(l => l with { Conditionnement = TrimOrEmpty(l.Conditionnement) })
            .ToList();

    private static string TrimOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}
