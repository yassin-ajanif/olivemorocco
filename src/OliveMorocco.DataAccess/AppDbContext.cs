using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Common
    public DbSet<Tiers> Tiers => Set<Tiers>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    // Operationnel
    public DbSet<Secteur> Secteurs => Set<Secteur>();
    public DbSet<SecteurVariete> SecteurVarietes => Set<SecteurVariete>();
    public DbSet<Variete> Varietes => Set<Variete>();
    public DbSet<Intrant> Intrants => Set<Intrant>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<InterventionLigne> InterventionLignes => Set<InterventionLigne>();
    public DbSet<Recolte> Recoltes => Set<Recolte>();
    public DbSet<Pressage> Pressages => Set<Pressage>();

    // Vente
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();
    public DbSet<DevisClient> DevisClients => Set<DevisClient>();
    public DbSet<DevisClientLigne> DevisClientLignes => Set<DevisClientLigne>();
    public DbSet<BonCommandeClient> BonsCommandeClient => Set<BonCommandeClient>();
    public DbSet<BonCommandeClientLigne> BonCommandeClientLignes => Set<BonCommandeClientLigne>();
    public DbSet<BonLivraisonClient> BonsLivraisonClient => Set<BonLivraisonClient>();
    public DbSet<BonLivraisonClientLigne> BonLivraisonClientLignes => Set<BonLivraisonClientLigne>();
    public DbSet<FactureClient> FacturesClient => Set<FactureClient>();
    public DbSet<FactureClientLigne> FactureClientLignes => Set<FactureClientLigne>();
    public DbSet<PaiementClient> PaiementsClient => Set<PaiementClient>();
    public DbSet<AvoirClient> AvoirsClient => Set<AvoirClient>();
    public DbSet<AvoirClientLigne> AvoirClientLignes => Set<AvoirClientLigne>();

    // Achat
    public DbSet<Service> Services => Set<Service>();
    public DbSet<TypeCharge> TypesCharge => Set<TypeCharge>();
    public DbSet<Charge> Charges => Set<Charge>();
    public DbSet<BonCommandeFournisseur> BonsCommandeFournisseur => Set<BonCommandeFournisseur>();
    public DbSet<BonCommandeFournisseurLigne> BonCommandeFournisseurLignes => Set<BonCommandeFournisseurLigne>();
    public DbSet<BonReception> BonsReception => Set<BonReception>();
    public DbSet<BonReceptionLigne> BonReceptionLignes => Set<BonReceptionLigne>();
    public DbSet<FactureFournisseur> FacturesFournisseur => Set<FactureFournisseur>();
    public DbSet<FactureFournisseurLigne> FactureFournisseurLignes => Set<FactureFournisseurLigne>();
    public DbSet<PaiementFournisseur> PaiementsFournisseur => Set<PaiementFournisseur>();
    public DbSet<AvoirFournisseur> AvoirsFournisseur => Set<AvoirFournisseur>();
    public DbSet<AvoirFournisseurLigne> AvoirFournisseurLignes => Set<AvoirFournisseurLigne>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplyDateTimeUtcConverters(modelBuilder);
    }

    private static void ApplyDateTimeUtcConverters(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => ToUtc(v),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? ToUtc(v.Value) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInfo();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
