using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.DataAccess;

public sealed class DatabaseInitializer(IServiceProvider services) : IAppDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
        await SeedDemoProduitsAsync(db, cancellationToken);
        await SeedDemoIntrantsAsync(db, cancellationToken);
        await SeedDemoSecteursAsync(db, cancellationToken);
        await SeedDemoFournisseursAsync(db, cancellationToken);
        await SeedDemoTypesChargesAsync(db, cancellationToken);
        await SeedDemoVarietesAsync(db, cancellationToken);
        await SeedDemoPressagesAsync(db, cancellationToken);
    }

    private static async Task SeedDemoProduitsAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Produits.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.Produits.AddRange(
            new Produit
            {
                Reference = "HVO-500",
                Designation = "Huile d'olive vierge extra 500 ml",
                Unite = "bouteille",
                PrixAchatHT = 45,
                PrixVenteHT = 75,
                TauxTVA = 20,
                StockActuel = 120,
                StockMinimum = 20,
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Produit
            {
                Reference = "HVO-1L",
                Designation = "Huile d'olive vierge extra 1 L",
                Unite = "bouteille",
                PrixAchatHT = 80,
                PrixVenteHT = 130,
                TauxTVA = 20,
                StockActuel = 80,
                StockMinimum = 15,
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Produit
            {
                Reference = "HVO-5L",
                Designation = "Huile d'olive vierge extra 5 L",
                Unite = "bidon",
                PrixAchatHT = 350,
                PrixVenteHT = 520,
                TauxTVA = 20,
                StockActuel = 40,
                StockMinimum = 8,
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoIntrantsAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Intrants.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.Intrants.AddRange(
            new Intrant
            {
                Nom = "Engrais NPK 15-15-15",
                Unite = "sac 25 kg",
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Intrant
            {
                Nom = "Compost organique",
                Unite = "tonne",
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Intrant
            {
                Nom = "Phytosanitaire cuivre",
                Unite = "L",
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Intrant
            {
                Nom = "Irrigation — tuyaux PE",
                Unite = "m",
                CreatedAt = now,
                UpdatedAt = now,
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoSecteursAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Secteurs.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.Secteurs.AddRange(
            new Secteur
            {
                Nom = "Secteur Nord",
                Code = "SN",
                SuperficieHectares = 15,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Secteur
            {
                Nom = "Secteur Est",
                Code = "SE",
                SuperficieHectares = 8,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Secteur
            {
                Nom = "Parcelle Sud",
                Code = "PS",
                SuperficieHectares = 5.5m,
                CreatedAt = now,
                UpdatedAt = now,
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoFournisseursAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Tiers.AnyAsync(t => t.Type == TypeTiers.Fournisseur || t.Type == TypeTiers.LesDeux, cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.Tiers.AddRange(
            new Tiers
            {
                Nom = "Huilerie Atlas",
                Type = TypeTiers.Fournisseur,
                Adresse = "Route de Fès, Km 8",
                Ville = "Meknès",
                Telephone = "+212 5 35 00 00 00",
                Email = "contact@huilerie-atlas.ma",
                ICE = "000000000000000",
                ConditionsPaiement = "30 jours fin de mois",
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Tiers
            {
                Nom = "Coopérative Oléicole Atlas",
                Type = TypeTiers.Fournisseur,
                Adresse = "Route de Fès, Km 12",
                Ville = "Meknès",
                Telephone = "+212 5 35 00 00 01",
                Email = "contact@coop-atlas.ma",
                ICE = "000000000000001",
                ConditionsPaiement = "30 jours fin de mois",
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Tiers
            {
                Nom = "Emballages Maroc SA",
                Type = TypeTiers.Fournisseur,
                Adresse = "Zone industrielle Aïn Sebaâ",
                Ville = "Casablanca",
                Telephone = "+212 5 22 00 00 02",
                Email = "achats@emballages-maroc.ma",
                ICE = "000000000000002",
                ConditionsPaiement = "45 jours",
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Tiers
            {
                Nom = "Matières Premières du Rif",
                Type = TypeTiers.Fournisseur,
                Adresse = "Bd Mohammed V",
                Ville = "Taza",
                Telephone = "+212 5 35 00 00 03",
                Email = "info@mp-rif.ma",
                ICE = "000000000000003",
                ConditionsPaiement = "Comptant",
                Actif = true,
                CreatedAt = now,
                UpdatedAt = now,
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoTypesChargesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.TypesCharge.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.TypesCharge.AddRange(
            new TypeCharge { Nom = "Main d'œuvre", Actif = true, CreatedAt = now, UpdatedAt = now },
            new TypeCharge { Nom = "Matériel", Actif = true, CreatedAt = now, UpdatedAt = now },
            new TypeCharge { Nom = "Transport", Actif = true, CreatedAt = now, UpdatedAt = now },
            new TypeCharge { Nom = "Autre", Actif = true, CreatedAt = now, UpdatedAt = now });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoVarietesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Varietes.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        db.Varietes.AddRange(
            new Variete
            {
                Nom = "Picholine Marocaine",
                Code = "PICH",
                RegionOrigine = "Fès-Meknès",
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Variete
            {
                Nom = "Haouzia",
                Code = "HAOU",
                RegionOrigine = "Marrakech-Safi",
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Variete
            {
                Nom = "Meslala",
                Code = "MESL",
                RegionOrigine = "Marrakech-Safi",
                CreatedAt = now,
                UpdatedAt = now,
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoPressagesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Pressages.AnyAsync(cancellationToken))
            return;

        var huilerie = await db.Tiers
            .FirstOrDefaultAsync(t => t.Nom == "Huilerie Atlas", cancellationToken)
            ?? await db.Tiers.FirstOrDefaultAsync(
                t => t.Type == TypeTiers.Fournisseur || t.Type == TypeTiers.LesDeux,
                cancellationToken);
        var picholine = await db.Varietes
            .FirstOrDefaultAsync(v => v.Code == "PICH", cancellationToken)
            ?? await db.Varietes.OrderBy(v => v.Id).FirstOrDefaultAsync(cancellationToken);

        if (huilerie is null || picholine is null)
            return;

        var now = DateTime.UtcNow;
        db.Pressages.Add(new Pressage
        {
            FournisseurId = huilerie.Id,
            VarieteId = picholine.Id,
            Date = new DateTime(2026, 11, 20),
            QuantiteOlives = 3200,
            Rendement = 17.5m,
            QuantiteHuile = 560,
            CreatedAt = now,
            UpdatedAt = now,
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
