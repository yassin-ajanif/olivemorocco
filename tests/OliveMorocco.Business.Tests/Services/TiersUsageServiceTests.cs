using FluentValidation;
using Moq;
using OliveMorocco.Business.Services;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Tests.Services;

[TestClass]
public sealed class TiersUsageServiceTests
{
    [TestMethod]
    public async Task EnsureCanDeleteClientAsync_succeeds_when_no_linked_docs()
    {
        var sut = CreateService();

        await sut.EnsureCanDeleteClientAsync(1);
    }

    [TestMethod]
    public async Task EnsureCanDeleteClientAsync_throws_when_devis_exists()
    {
        var devis = new Mock<IRepository<DevisClient>>();
        devis.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DevisClient, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = CreateService(devisClients: devis.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationException>(
            () => sut.EnsureCanDeleteClientAsync(1));

        StringAssert.Contains(ex.Message, "Devis client");
        StringAssert.Contains(ex.Message, "Désactivez-le");
    }

    private static TiersUsageService CreateService(
        IRepository<DevisClient>? devisClients = null,
        IRepository<BonCommandeClient>? bonsCommandeClients = null,
        IRepository<BonLivraisonClient>? bonsLivraisonClients = null,
        IRepository<FactureClient>? facturesClients = null,
        IRepository<AvoirClient>? avoirsClients = null,
        IRepository<BonCommandeFournisseur>? bonsCommandeFournisseur = null,
        IRepository<BonReception>? bonsReception = null,
        IRepository<FactureFournisseur>? facturesFournisseur = null,
        IRepository<AvoirFournisseur>? avoirsFournisseur = null,
        IRepository<Pressage>? pressages = null)
    {
        return new TiersUsageService(
            devisClients ?? EmptyRepo<DevisClient>(),
            bonsCommandeClients ?? EmptyRepo<BonCommandeClient>(),
            bonsLivraisonClients ?? EmptyRepo<BonLivraisonClient>(),
            facturesClients ?? EmptyRepo<FactureClient>(),
            avoirsClients ?? EmptyRepo<AvoirClient>(),
            bonsCommandeFournisseur ?? EmptyRepo<BonCommandeFournisseur>(),
            bonsReception ?? EmptyRepo<BonReception>(),
            facturesFournisseur ?? EmptyRepo<FactureFournisseur>(),
            avoirsFournisseur ?? EmptyRepo<AvoirFournisseur>(),
            pressages ?? EmptyRepo<Pressage>());
    }

    private static IRepository<T> EmptyRepo<T>() where T : Domain.Common.BaseEntity
    {
        var mock = new Mock<IRepository<T>>();
        mock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return mock.Object;
    }
}
