using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.DataAccess;

public sealed class DatabaseInitializer(IServiceProvider services) : IAppDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
        await SeedDemoProduitsAsync(db, cancellationToken);
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
}
