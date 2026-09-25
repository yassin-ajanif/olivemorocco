using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Web.Models.Achat.Charges;

public sealed class ChargeFormViewModel
{
    public int? Id { get; set; }

    public int TypeChargeId { get; set; }

    public string Libelle { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public decimal MontantTtc { get; set; }

    public string? Note { get; set; }

    public IReadOnlyList<TypeChargeSelectItemDto> TypeCharges { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
