using FluentValidation;
using Moq;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Business.Tests.Helpers;
using OliveMorocco.Business.Validation.Vente;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Tests.Services.Vente;

[TestClass]
public sealed class ClientServiceTests
{
    private Mock<IRepository<Tiers>> _repo = null!;
    private Mock<ITiersUsageService> _tiersUsage = null!;
    private ClientService _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _repo = new Mock<IRepository<Tiers>>();
        _tiersUsage = new Mock<ITiersUsageService>();

        _sut = new ClientService(
            _repo.Object,
            _tiersUsage.Object,
            MapperFactory.Create(),
            new IValidator<CreateClientDto>[] { new CreateClientDtoValidator() },
            new IValidator<UpdateClientDto>[] { new UpdateClientDtoValidator() });
    }

    [TestMethod]
    public async Task GetClientByIdAsync_returns_null_for_fournisseur_only()
    {
        var fournisseur = new Tiers { Id = 1, Nom = "Fournisseur", Type = TypeTiers.Fournisseur };
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(fournisseur);

        var result = await _sut.GetClientByIdAsync(1);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetClientByIdAsync_returns_client_dto()
    {
        var client = new Tiers
        {
            Id = 2,
            Nom = "Client A",
            Type = TypeTiers.Client,
            Ville = "Tétouan",
            Actif = true
        };
        _repo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var result = await _sut.GetClientByIdAsync(2);

        Assert.IsNotNull(result);
        Assert.AreEqual("Client A", result!.Nom);
        Assert.AreEqual(TypeTiers.Client, result.Type);
    }

    [TestMethod]
    public async Task CreateClientAsync_sets_type_to_client()
    {
        var dto = new CreateClientDto("New Client", "", "Rabat", "", "", "", "", true);
        _repo.Setup(r => r.AddAsync(It.IsAny<Tiers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tiers t, CancellationToken _) =>
            {
                t.Id = 10;
                return t;
            });

        var result = await _sut.CreateClientAsync(dto);

        Assert.AreEqual(TypeTiers.Client, result.Type);
        Assert.AreEqual("New Client", result.Nom);
        _repo.Verify(r => r.AddAsync(
            It.Is<Tiers>(t => t.Type == TypeTiers.Client && t.Nom == "New Client"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateClientAsync_throws_when_nom_empty()
    {
        var dto = new CreateClientDto("", "", "", "", "", "", "", true);

        await Assert.ThrowsExceptionAsync<ValidationException>(
            () => _sut.CreateClientAsync(dto));
    }

    [TestMethod]
    public async Task ToggleActifAsync_flips_actif_flag()
    {
        var client = new Tiers
        {
            Id = 3,
            Nom = "Toggle Me",
            Type = TypeTiers.Client,
            Actif = true
        };

        _repo.SetupSequence(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client)           // GetClientByIdAsync
            .ReturnsAsync(client)           // UpdateAsync load
            .ReturnsAsync(() =>
            {
                client.Actif = false;
                return client;
            });                             // final GetByIdAsync

        _repo.Setup(r => r.UpdateAsync(It.IsAny<Tiers>(), It.IsAny<CancellationToken>()))
            .Callback<Tiers, CancellationToken>((t, _) => client.Actif = t.Actif)
            .Returns(Task.CompletedTask);

        var result = await _sut.ToggleActifAsync(3);

        Assert.IsFalse(result.Actif);
        _repo.Verify(r => r.UpdateAsync(
            It.Is<Tiers>(t => t.Id == 3 && !t.Actif),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteClientAsync_calls_tiers_usage_guard()
    {
        var client = new Tiers { Id = 4, Nom = "Protected", Type = TypeTiers.Client };
        _repo.Setup(r => r.GetByIdAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        await _sut.DeleteClientAsync(4);

        _tiersUsage.Verify(
            s => s.EnsureCanDeleteClientAsync(4, It.IsAny<CancellationToken>()),
            Times.Once);
        _repo.Verify(r => r.DeleteAsync(4, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteClientAsync_throws_when_tiers_usage_blocks()
    {
        var client = new Tiers { Id = 5, Nom = "Linked", Type = TypeTiers.Client };
        _repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _tiersUsage
            .Setup(s => s.EnsureCanDeleteClientAsync(5, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("blocked"));

        await Assert.ThrowsExceptionAsync<ValidationException>(
            () => _sut.DeleteClientAsync(5));

        _repo.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
