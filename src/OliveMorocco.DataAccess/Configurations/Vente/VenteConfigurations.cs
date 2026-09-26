using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.DataAccess.Configurations.Vente;

public class ProduitConfiguration : IEntityTypeConfiguration<Produit>
{
    public void Configure(EntityTypeBuilder<Produit> builder)
    {
        builder.ToTable("Produits");

        builder.HasIndex(p => p.Reference).IsUnique();
        builder.HasIndex(p => p.VarieteId);

        builder.Property(p => p.Reference).IsRequired();
        builder.Property(p => p.Designation).IsRequired();
        builder.Property(p => p.Unite).IsRequired();
        builder.Property(p => p.PrixAchatHT).HasPrecision(18, 2);
        builder.Property(p => p.PrixVenteHT).HasPrecision(18, 2);
        builder.Property(p => p.TauxTVA).HasPrecision(18, 2);
        builder.Property(p => p.StockActuel).HasPrecision(12, 4);
        builder.Property(p => p.StockMinimum).HasPrecision(12, 4);

        builder.HasOne(p => p.Variete)
            .WithMany(v => v.Produits)
            .HasForeignKey(p => p.VarieteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class MouvementStockConfiguration : IEntityTypeConfiguration<MouvementStock>
{
    public void Configure(EntityTypeBuilder<MouvementStock> builder)
    {
        builder.ToTable("MouvementsStock", t => t.HasCheckConstraint(
            "CK_MouvementsStock_Type",
            "\"Type\" IN ('Entree','Sortie','Ajustement')"));

        builder.HasIndex(m => m.ProduitId);

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Quantite).HasPrecision(12, 4);
        builder.Property(m => m.StockAvant).HasPrecision(12, 4);
        builder.Property(m => m.OrigineType).IsRequired();
        builder.Property(m => m.Note).IsRequired();

        builder.HasOne(m => m.Produit)
            .WithMany(p => p.MouvementsStock)
            .HasForeignKey(m => m.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DevisClientConfiguration : IEntityTypeConfiguration<DevisClient>
{
    public void Configure(EntityTypeBuilder<DevisClient> builder)
    {
        builder.ToTable("Devis");

        builder.HasIndex(d => d.Numero).IsUnique();

        builder.Property(d => d.Numero).IsRequired();
        builder.Property(d => d.RemiseGlobale).HasPrecision(18, 2);
        builder.Property(d => d.Note).IsRequired();

        builder.HasOne(d => d.Client)
            .WithMany(t => t.DevisClients)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Lignes)
            .WithOne(l => l.DevisClient)
            .HasForeignKey(l => l.DevisClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DevisClientLigneConfiguration : IEntityTypeConfiguration<DevisClientLigne>
{
    public void Configure(EntityTypeBuilder<DevisClientLigne> builder)
    {
        builder.ToTable("DevisLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.Quantite).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.DevisClientLignes)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BonCommandeClientConfiguration : IEntityTypeConfiguration<BonCommandeClient>
{
    public void Configure(EntityTypeBuilder<BonCommandeClient> builder)
    {
        builder.ToTable("BonsCommandeClient");

        builder.HasIndex(b => b.Numero).IsUnique();
        builder.HasIndex(b => b.FactureId);

        builder.Property(b => b.Numero).IsRequired();
        builder.Property(b => b.Note).IsRequired();

        builder.HasOne(b => b.Client)
            .WithMany(t => t.BonsCommandeClient)
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Devis)
            .WithMany(d => d.BonsCommandeClient)
            .HasForeignKey(b => b.DevisId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Facture)
            .WithMany(f => f.BonsCommandeClient)
            .HasForeignKey(b => b.FactureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.Lignes)
            .WithOne(l => l.BonCommandeClient)
            .HasForeignKey(l => l.BonCommandeClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BonCommandeClientLigneConfiguration : IEntityTypeConfiguration<BonCommandeClientLigne>
{
    public void Configure(EntityTypeBuilder<BonCommandeClientLigne> builder)
    {
        builder.ToTable("BonCommandeClientLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.QuantiteCommandee).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.BonCommandeClientLignes)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BonLivraisonClientConfiguration : IEntityTypeConfiguration<BonLivraisonClient>
{
    public void Configure(EntityTypeBuilder<BonLivraisonClient> builder)
    {
        builder.ToTable("BonsLivraison");

        builder.HasIndex(b => b.Numero).IsUnique();
        builder.HasIndex(b => b.BonCommandeClientId);
        builder.HasIndex(b => b.FactureId);

        builder.Property(b => b.Numero).IsRequired();
        builder.Property(b => b.Note).IsRequired();

        builder.HasOne(b => b.Client)
            .WithMany(t => t.BonsLivraisonClient)
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Devis)
            .WithMany(d => d.BonsLivraisonClient)
            .HasForeignKey(b => b.DevisId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.BonCommandeClient)
            .WithMany(c => c.BonsLivraisonClient)
            .HasForeignKey(b => b.BonCommandeClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Facture)
            .WithMany(f => f.BonsLivraisonClient)
            .HasForeignKey(b => b.FactureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.Lignes)
            .WithOne(l => l.BonLivraisonClient)
            .HasForeignKey(l => l.BLId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BonLivraisonClientLigneConfiguration : IEntityTypeConfiguration<BonLivraisonClientLigne>
{
    public void Configure(EntityTypeBuilder<BonLivraisonClientLigne> builder)
    {
        builder.ToTable("BonLivraisonLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.QuantiteCommandee).HasPrecision(12, 4);
        builder.Property(l => l.QuantiteLivree).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.BonLivraisonClientLignes)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FactureClientConfiguration : IEntityTypeConfiguration<FactureClient>
{
    public void Configure(EntityTypeBuilder<FactureClient> builder)
    {
        builder.ToTable("Factures");

        builder.HasIndex(f => f.Numero).IsUnique();

        builder.Property(f => f.Numero).IsRequired();
        builder.Property(f => f.BonCommandeReference).IsRequired();
        builder.Property(f => f.RemiseGlobale).HasPrecision(18, 2);
        builder.Property(f => f.TotalTtc).HasPrecision(18, 2);
        builder.Property(f => f.Note).IsRequired();

        builder.HasOne(f => f.Client)
            .WithMany(t => t.FacturesClient)
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Devis)
            .WithMany(d => d.FacturesClient)
            .HasForeignKey(f => f.DevisId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(f => f.Lignes)
            .WithOne(l => l.FactureClient)
            .HasForeignKey(l => l.FactureClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Paiements)
            .WithOne(p => p.FactureClient)
            .HasForeignKey(p => p.FactureClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FactureClientLigneConfiguration : IEntityTypeConfiguration<FactureClientLigne>
{
    public void Configure(EntityTypeBuilder<FactureClientLigne> builder)
    {
        builder.ToTable("FactureLignes");

        builder.HasIndex(l => l.FactureClientId);
        builder.HasIndex(l => l.BonLivraisonId);

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.Quantite).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.BonLivraison)
            .WithMany(b => b.FactureClientLignes)
            .HasForeignKey(l => l.BonLivraisonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.FactureClientLignes)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaiementClientConfiguration : IEntityTypeConfiguration<PaiementClient>
{
    public void Configure(EntityTypeBuilder<PaiementClient> builder)
    {
        builder.ToTable("Paiements", t => t.HasCheckConstraint(
            "CK_Paiements_Mode",
            "\"Mode\" IN ('Cheque','Especes','TPE','Virement','Effet')"));

        builder.HasIndex(p => p.FactureClientId);

        builder.Property(p => p.Mode)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Montant).HasPrecision(18, 2);
        builder.Property(p => p.Reference).IsRequired();
    }
}

public class AvoirClientConfiguration : IEntityTypeConfiguration<AvoirClient>
{
    public void Configure(EntityTypeBuilder<AvoirClient> builder)
    {
        builder.ToTable("Avoirs");

        builder.HasIndex(a => a.Numero).IsUnique();
        builder.HasIndex(a => a.FactureId);

        builder.Property(a => a.Numero).IsRequired();
        builder.Property(a => a.Motif).IsRequired();

        builder.HasOne(a => a.Client)
            .WithMany(t => t.AvoirsClient)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Facture)
            .WithMany(f => f.AvoirsClient)
            .HasForeignKey(a => a.FactureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(a => a.Lignes)
            .WithOne(l => l.AvoirClient)
            .HasForeignKey(l => l.AvoirClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AvoirClientLigneConfiguration : IEntityTypeConfiguration<AvoirClientLigne>
{
    public void Configure(EntityTypeBuilder<AvoirClientLigne> builder)
    {
        builder.ToTable("AvoirLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.Quantite).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.AvoirClientLignes)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
