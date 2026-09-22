using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.DataAccess.Configurations.Common;

public class TiersConfiguration : IEntityTypeConfiguration<Tiers>
{
    public void Configure(EntityTypeBuilder<Tiers> builder)
    {
        builder.ToTable("Tiers", t => t.HasCheckConstraint(
            "CK_Tiers_Type",
            "\"Type\" IN ('Client','Fournisseur','LesDeux')"));

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Nom).IsRequired();
        builder.Property(t => t.Adresse).IsRequired();
        builder.Property(t => t.Ville).IsRequired();
        builder.Property(t => t.Telephone).IsRequired();
        builder.Property(t => t.Email).IsRequired();
        builder.Property(t => t.ICE).IsRequired();
        builder.Property(t => t.ConditionsPaiement).IsRequired();
    }
}
