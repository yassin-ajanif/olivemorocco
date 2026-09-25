using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.DataAccess.Configurations.Achat;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasIndex(s => s.Nom).IsUnique();
        builder.HasIndex(s => s.Reference).IsUnique();

        builder.Property(s => s.Reference).HasMaxLength(32);
        builder.Property(s => s.Nom).HasMaxLength(128).IsRequired();
        builder.Property(s => s.Unite).HasMaxLength(16).IsRequired();
        builder.Property(s => s.PrixAchatHT).HasPrecision(18, 2);
        builder.Property(s => s.TauxTVA).HasPrecision(5, 2);
    }
}

public class TypeChargeConfiguration : IEntityTypeConfiguration<TypeCharge>
{
    public void Configure(EntityTypeBuilder<TypeCharge> builder)
    {
        builder.ToTable("TypesCharges");

        builder.HasIndex(t => t.Nom).IsUnique();

        builder.Property(t => t.Nom).HasMaxLength(128).IsRequired();
    }
}

public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.ToTable("Charges");

        builder.HasIndex(c => c.TypeChargeId);
        builder.HasIndex(c => c.InterventionId);
        builder.HasIndex(c => c.Date);

        builder.Property(c => c.Libelle).HasMaxLength(256).IsRequired();
        builder.Property(c => c.MontantTtc).HasPrecision(18, 2);
        builder.Property(c => c.Note).IsRequired();

        builder.HasOne(c => c.TypeCharge)
            .WithMany(t => t.Charges)
            .HasForeignKey(c => c.TypeChargeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Intervention)
            .WithMany(i => i.Charges)
            .HasForeignKey(c => c.InterventionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class BonCommandeFournisseurConfiguration : IEntityTypeConfiguration<BonCommandeFournisseur>
{
    public void Configure(EntityTypeBuilder<BonCommandeFournisseur> builder)
    {
        builder.ToTable("BonsCommande");

        builder.HasIndex(b => b.Numero).IsUnique();

        builder.Property(b => b.Numero).IsRequired();
        builder.Property(b => b.Note).IsRequired();

        builder.HasOne(b => b.Fournisseur)
            .WithMany(t => t.BonsCommandeFournisseur)
            .HasForeignKey(b => b.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Lignes)
            .WithOne(l => l.BonCommandeFournisseur)
            .HasForeignKey(l => l.BonCommandeFournisseurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BonCommandeFournisseurLigneConfiguration : IEntityTypeConfiguration<BonCommandeFournisseurLigne>
{
    public void Configure(EntityTypeBuilder<BonCommandeFournisseurLigne> builder)
    {
        builder.ToTable("BonCommandeLignes", t => t.HasCheckConstraint(
            "CK_BonCommandeLignes_IntrantOrService",
            "(\"IntrantId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"IntrantId\" IS NULL AND \"ServiceId\" IS NOT NULL)"));

        builder.HasIndex(l => l.BonCommandeFournisseurId);
        builder.HasIndex(l => l.IntrantId);
        builder.HasIndex(l => l.ServiceId);

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.QuantiteCommandee).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.Intrant)
            .WithMany()
            .HasForeignKey(l => l.IntrantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Service)
            .WithMany(s => s.BonCommandeFournisseurLignes)
            .HasForeignKey(l => l.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BonReceptionConfiguration : IEntityTypeConfiguration<BonReception>
{
    public void Configure(EntityTypeBuilder<BonReception> builder)
    {
        builder.ToTable("BonsReception");

        builder.HasIndex(b => b.Numero).IsUnique();
        builder.HasIndex(b => b.BonCommandeId);
        builder.HasIndex(b => b.FactureFournisseurId);

        builder.Property(b => b.Numero).IsRequired();
        builder.Property(b => b.TotalTtc).HasPrecision(18, 2);
        builder.Property(b => b.Note).IsRequired();

        builder.HasOne(b => b.Fournisseur)
            .WithMany(t => t.BonsReception)
            .HasForeignKey(b => b.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.BonCommande)
            .WithMany(c => c.BonsReception)
            .HasForeignKey(b => b.BonCommandeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.FactureFournisseur)
            .WithMany(f => f.BonsReception)
            .HasForeignKey(b => b.FactureFournisseurId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.Lignes)
            .WithOne(l => l.BonReception)
            .HasForeignKey(l => l.BRId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BonReceptionLigneConfiguration : IEntityTypeConfiguration<BonReceptionLigne>
{
    public void Configure(EntityTypeBuilder<BonReceptionLigne> builder)
    {
        builder.ToTable("BonReceptionLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.QuantiteRecue).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasIndex(l => l.IntrantId);

        builder.HasOne(l => l.Intrant)
            .WithMany()
            .HasForeignKey(l => l.IntrantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FactureFournisseurConfiguration : IEntityTypeConfiguration<FactureFournisseur>
{
    public void Configure(EntityTypeBuilder<FactureFournisseur> builder)
    {
        builder.ToTable("FacturesFournisseurs");

        builder.HasIndex(f => f.Numero).IsUnique();

        builder.Property(f => f.Numero).IsRequired();
        builder.Property(f => f.RemiseGlobale).HasPrecision(18, 2);
        builder.Property(f => f.TotalTtc).HasPrecision(18, 2);
        builder.Property(f => f.Note).IsRequired();

        builder.HasOne(f => f.Fournisseur)
            .WithMany(t => t.FacturesFournisseur)
            .HasForeignKey(f => f.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Lignes)
            .WithOne(l => l.FactureFournisseur)
            .HasForeignKey(l => l.FactureFournisseurId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Paiements)
            .WithOne(p => p.FactureFournisseur)
            .HasForeignKey(p => p.FactureFournisseurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FactureFournisseurLigneConfiguration : IEntityTypeConfiguration<FactureFournisseurLigne>
{
    public void Configure(EntityTypeBuilder<FactureFournisseurLigne> builder)
    {
        builder.ToTable("FactureFournisseurLignes", t => t.HasCheckConstraint(
            "CK_FactureFournisseurLignes_IntrantOrService",
            "(\"IntrantId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"IntrantId\" IS NULL AND \"ServiceId\" IS NOT NULL)"));

        builder.HasIndex(l => l.FactureFournisseurId);
        builder.HasIndex(l => l.BonReceptionId);
        builder.HasIndex(l => l.IntrantId);
        builder.HasIndex(l => l.ServiceId);

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.Quantite).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasOne(l => l.BonReception)
            .WithMany(b => b.FactureFournisseurLignes)
            .HasForeignKey(l => l.BonReceptionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(l => l.Intrant)
            .WithMany()
            .HasForeignKey(l => l.IntrantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Service)
            .WithMany(s => s.FactureFournisseurLignes)
            .HasForeignKey(l => l.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaiementFournisseurConfiguration : IEntityTypeConfiguration<PaiementFournisseur>
{
    public void Configure(EntityTypeBuilder<PaiementFournisseur> builder)
    {
        builder.ToTable("PaiementsFournisseurs", t => t.HasCheckConstraint(
            "CK_PaiementsFournisseurs_Mode",
            "\"Mode\" IN ('Credit','Cheque','Especes','TPE','Virement','Effet')"));

        builder.HasIndex(p => p.FactureFournisseurId);

        builder.Property(p => p.Mode)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Montant).HasPrecision(18, 2);
        builder.Property(p => p.Reference).IsRequired();
    }
}

public class AvoirFournisseurConfiguration : IEntityTypeConfiguration<AvoirFournisseur>
{
    public void Configure(EntityTypeBuilder<AvoirFournisseur> builder)
    {
        builder.ToTable("AvoirsFournisseurs");

        builder.HasIndex(a => a.Numero).IsUnique();

        builder.Property(a => a.Numero).IsRequired();
        builder.Property(a => a.Motif).IsRequired();

        builder.HasOne(a => a.Fournisseur)
            .WithMany(t => t.AvoirsFournisseur)
            .HasForeignKey(a => a.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Lignes)
            .WithOne(l => l.AvoirFournisseur)
            .HasForeignKey(l => l.AvoirFournisseurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AvoirFournisseurLigneConfiguration : IEntityTypeConfiguration<AvoirFournisseurLigne>
{
    public void Configure(EntityTypeBuilder<AvoirFournisseurLigne> builder)
    {
        builder.ToTable("AvoirFournisseurLignes");

        builder.Property(l => l.Designation).IsRequired();
        builder.Property(l => l.Conditionnement).IsRequired();
        builder.Property(l => l.Quantite).HasPrecision(12, 4);
        builder.Property(l => l.PrixUnitaireHT).HasPrecision(18, 2);
        builder.Property(l => l.Remise).HasPrecision(18, 2);
        builder.Property(l => l.TauxTVA).HasPrecision(18, 2);

        builder.HasIndex(l => l.IntrantId);

        builder.HasOne(l => l.Intrant)
            .WithMany()
            .HasForeignKey(l => l.IntrantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
