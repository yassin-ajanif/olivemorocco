using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Achat;

internal static class AchatLineReference
{
    /// <summary>
    /// Display value for the document line "Réf." column (intrant unit on achat forms).
    /// </summary>
    public static string FromIntrant(Intrant? intrant) =>
        intrant?.Unite ?? string.Empty;

    public static string FromIntrantOrService(Intrant? intrant, Service? service) =>
        intrant is not null
            ? intrant.Unite
            : service?.Reference ?? service?.Unite ?? string.Empty;
}
