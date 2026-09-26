using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.DataAccess.Configurations.Operationnel;

public class SecteurConfiguration : IEntityTypeConfiguration<Secteur>
{
    public void Configure(EntityTypeBuilder<Secteur> builder)
    {
        builder.ToTable("Secteurs");

        builder.HasIndex(s => s.Nom).IsUnique();
        builder.HasIndex(s => s.Code).IsUnique();

        builder.Property(s => s.Nom).HasMaxLength(128).IsRequired();
        builder.Property(s => s.Code).HasMaxLength(32);
        builder.Property(s => s.SuperficieHectares).HasPrecision(10, 4);
    }
}

public class SecteurVarieteConfiguration : IEntityTypeConfiguration<SecteurVariete>
{
    public void Configure(EntityTypeBuilder<SecteurVariete> builder)
    {
        builder.ToTable("SecteurVarietes");

        builder.HasIndex(sv => new { sv.SecteurId, sv.VarieteId }).IsUnique();
        builder.HasIndex(sv => sv.VarieteId);

        builder.Property(sv => sv.SuperficieHectares).HasPrecision(10, 4);

        builder.HasOne(sv => sv.Secteur)
            .WithMany(s => s.SecteurVarietes)
            .HasForeignKey(sv => sv.SecteurId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sv => sv.Variete)
            .WithMany(v => v.SecteurVarietes)
            .HasForeignKey(sv => sv.VarieteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VarieteConfiguration : IEntityTypeConfiguration<Variete>
{
    public void Configure(EntityTypeBuilder<Variete> builder)
    {
        builder.ToTable("Varietes");

        builder.HasIndex(v => v.Nom).IsUnique();
        builder.HasIndex(v => v.Code).IsUnique();

        builder.Property(v => v.Nom).HasMaxLength(128).IsRequired();
        builder.Property(v => v.Code).HasMaxLength(32);
        builder.Property(v => v.RegionOrigine).HasMaxLength(128);
    }
}

public class IntrantConfiguration : IEntityTypeConfiguration<Intrant>
{
    public void Configure(EntityTypeBuilder<Intrant> builder)
    {
        builder.ToTable("Intrants");

        builder.HasIndex(i => i.Nom).IsUnique();

        builder.Property(i => i.Nom).HasMaxLength(128).IsRequired();
        builder.Property(i => i.Unite).HasMaxLength(16).IsRequired();
        builder.Property(i => i.PrixAchatHT).HasPrecision(18, 2);
    }
}

public class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> builder)
    {
        builder.ToTable("Interventions");

        builder.HasIndex(i => i.SecteurId);
        builder.HasIndex(i => i.Date);

        builder.Property(i => i.QuantiteEau).HasPrecision(12, 4);

        builder.HasOne(i => i.Secteur)
            .WithMany(s => s.Interventions)
            .HasForeignKey(i => i.SecteurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Lignes)
            .WithOne(l => l.Intervention)
            .HasForeignKey(l => l.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Charges)
            .WithOne(c => c.Intervention)
            .HasForeignKey(c => c.InterventionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class InterventionLigneConfiguration : IEntityTypeConfiguration<InterventionLigne>
{
    public void Configure(EntityTypeBuilder<InterventionLigne> builder)
    {
        builder.ToTable("InterventionLignes");

        builder.HasIndex(l => l.InterventionId);
        builder.HasIndex(l => l.IntrantId);
        builder.HasIndex(l => new { l.InterventionId, l.IntrantId }).IsUnique();

        builder.Property(l => l.Quantite).HasPrecision(12, 4);

        builder.HasOne(l => l.Intrant)
            .WithMany(i => i.InterventionLignes)
            .HasForeignKey(l => l.IntrantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RecolteConfiguration : IEntityTypeConfiguration<Recolte>
{
    public void Configure(EntityTypeBuilder<Recolte> builder)
    {
        builder.ToTable("Recoltes");

        builder.HasIndex(r => r.SecteurId);
        builder.HasIndex(r => r.VarieteId);
        builder.HasIndex(r => r.Date);

        builder.Property(r => r.Quantite).HasPrecision(12, 4);

        builder.HasOne(r => r.Secteur)
            .WithMany(s => s.Recoltes)
            .HasForeignKey(r => r.SecteurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Variete)
            .WithMany(v => v.Recoltes)
            .HasForeignKey(r => r.VarieteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PressageConfiguration : IEntityTypeConfiguration<Pressage>
{
    public void Configure(EntityTypeBuilder<Pressage> builder)
    {
        builder.ToTable("Pressages");

        builder.HasIndex(p => p.FournisseurId);
        builder.HasIndex(p => p.VarieteId);
        builder.HasIndex(p => p.Date);
        builder.HasIndex(p => p.FactureFournisseurId);

        builder.Property(p => p.QuantiteOlives).HasPrecision(12, 4);
        builder.Property(p => p.Rendement).HasPrecision(5, 2);
        builder.Property(p => p.QuantiteHuile).HasPrecision(12, 4);

        builder.HasOne(p => p.Fournisseur)
            .WithMany(t => t.Pressages)
            .HasForeignKey(p => p.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Variete)
            .WithMany(v => v.Pressages)
            .HasForeignKey(p => p.VarieteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.FactureFournisseur)
            .WithMany(f => f.Pressages)
            .HasForeignKey(p => p.FactureFournisseurId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
