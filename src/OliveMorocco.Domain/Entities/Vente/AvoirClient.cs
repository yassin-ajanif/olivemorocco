using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class AvoirClient : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public int? FactureId { get; set; }
    public string Motif { get; set; } = string.Empty;
    public bool RetourMarchandise { get; set; }

    public Tiers Client { get; set; } = null!;
    public FactureClient? Facture { get; set; }
    public ICollection<AvoirClientLigne> Lignes { get; set; } = new List<AvoirClientLigne>();
}
