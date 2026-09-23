using System.Text;
using FluentValidation;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services;

public sealed class TiersUsageService(
    IRepository<DevisClient> devisClients,
    IRepository<BonCommandeClient> bonsCommandeClients,
    IRepository<BonLivraisonClient> bonsLivraisonClients,
    IRepository<FactureClient> facturesClients,
    IRepository<AvoirClient> avoirsClients,
    IRepository<BonCommandeFournisseur> bonsCommandeFournisseur,
    IRepository<BonReception> bonsReception,
    IRepository<FactureFournisseur> facturesFournisseur,
    IRepository<AvoirFournisseur> avoirsFournisseur,
    IRepository<Pressage> pressages) : ITiersUsageService
{
    public async Task EnsureCanDeleteClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var linked = await GetLinkedDocumentLabelsAsync(clientId, cancellationToken);
        if (linked.Count == 0)
            return;

        throw new ValidationException(BuildDeleteBlockedMessage("client", linked));
    }

    public async Task EnsureCanDeleteFournisseurAsync(int fournisseurId, CancellationToken cancellationToken = default)
    {
        var linked = await GetLinkedFournisseurDocumentLabelsAsync(fournisseurId, cancellationToken);
        if (linked.Count == 0)
            return;

        throw new ValidationException(BuildDeleteBlockedMessage("fournisseur", linked));
    }

    private async Task<List<string>> GetLinkedDocumentLabelsAsync(int tiersId, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await devisClients.AnyAsync(d => d.ClientId == tiersId, cancellationToken))
            linked.Add("Devis client");
        if (await bonsCommandeClients.AnyAsync(b => b.ClientId == tiersId, cancellationToken))
            linked.Add("Bons de commande client");
        if (await bonsLivraisonClients.AnyAsync(b => b.ClientId == tiersId, cancellationToken))
            linked.Add("Bons de livraison client");
        if (await facturesClients.AnyAsync(f => f.ClientId == tiersId, cancellationToken))
            linked.Add("Factures client");
        if (await avoirsClients.AnyAsync(a => a.ClientId == tiersId, cancellationToken))
            linked.Add("Avoirs client");
        if (await bonsCommandeFournisseur.AnyAsync(b => b.FournisseurId == tiersId, cancellationToken))
            linked.Add("Bons de commande fournisseur");
        if (await bonsReception.AnyAsync(b => b.FournisseurId == tiersId, cancellationToken))
            linked.Add("Bons de réception");
        if (await facturesFournisseur.AnyAsync(f => f.FournisseurId == tiersId, cancellationToken))
            linked.Add("Factures fournisseur");
        if (await avoirsFournisseur.AnyAsync(a => a.FournisseurId == tiersId, cancellationToken))
            linked.Add("Avoirs fournisseur");
        if (await pressages.AnyAsync(p => p.FournisseurId == tiersId, cancellationToken))
            linked.Add("Pressages");

        return linked;
    }

    private async Task<List<string>> GetLinkedFournisseurDocumentLabelsAsync(int tiersId, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await bonsCommandeFournisseur.AnyAsync(b => b.FournisseurId == tiersId, cancellationToken))
            linked.Add("Bons de commande fournisseur");
        if (await bonsReception.AnyAsync(b => b.FournisseurId == tiersId, cancellationToken))
            linked.Add("Bons de réception");
        if (await facturesFournisseur.AnyAsync(f => f.FournisseurId == tiersId, cancellationToken))
            linked.Add("Factures fournisseur");
        if (await avoirsFournisseur.AnyAsync(a => a.FournisseurId == tiersId, cancellationToken))
            linked.Add("Avoirs fournisseur");
        if (await pressages.AnyAsync(p => p.FournisseurId == tiersId, cancellationToken))
            linked.Add("Pressages");

        return linked;
    }

    private static string BuildDeleteBlockedMessage(string tiersKind, IReadOnlyList<string> linked)
    {
        var message = new StringBuilder();
        message.AppendLine($"Impossible de supprimer ce {tiersKind}.");
        message.AppendLine();
        message.AppendLine("Documents liés :");
        foreach (var label in linked)
            message.AppendLine($"• {label}");
        message.AppendLine();
        message.Append("Désactivez-le à la place.");
        return message.ToString().TrimEnd();
    }
}
