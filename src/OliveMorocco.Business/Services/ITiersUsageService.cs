namespace OliveMorocco.Business.Services;

public interface ITiersUsageService
{
    /// <summary>
    /// Throws if the tier row is referenced by documents (client, fournisseur, or pressage).
    /// Delete removes the whole <see cref="Domain.Entities.Common.Tiers"/> row.
    /// </summary>
    Task EnsureCanDeleteClientAsync(int clientId, CancellationToken cancellationToken = default);
}
