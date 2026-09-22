using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.DataAccess.Configurations.Common;

public class AppSettingsConfiguration : IEntityTypeConfiguration<AppSettings>
{
    public void Configure(EntityTypeBuilder<AppSettings> builder)
    {
        builder.ToTable("AppSettings");

        builder.Property(s => s.SocieteNom).IsRequired();
        builder.Property(s => s.SocieteAdresse).IsRequired();
        builder.Property(s => s.SocieteICE).IsRequired();
        builder.Property(s => s.Devise).IsRequired();
        builder.Property(s => s.TauxTVAJson).IsRequired();
        builder.Property(s => s.DocumentNumberingFloorsJson).IsRequired();
        builder.Property(s => s.UiLanguage).IsRequired();
        builder.Property(s => s.BackupDirectory).IsRequired();
        builder.Property(s => s.BackupIntervalUnit).IsRequired();
    }
}
