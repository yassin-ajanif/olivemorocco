using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Stockage.Secteurs;

public sealed class SecteurFormViewModel
{
    public int? Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string? Code { get; set; }

    public decimal SuperficieHectares { get; set; }

    public List<SecteurVarieteLineViewModel> Lignes { get; set; } = [];

    public IReadOnlyList<VarieteSelectItemDto> Varietes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
