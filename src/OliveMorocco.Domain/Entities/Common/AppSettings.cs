namespace OliveMorocco.Domain.Entities.Common;

public class AppSettings
{
    public int Id { get; set; }
    public string SocieteNom { get; set; } = string.Empty;
    public string SocieteAdresse { get; set; } = string.Empty;
    public string SocieteICE { get; set; } = string.Empty;
    public string? SocieteLogoPath { get; set; }
    public string? SocieteMentionsLegales { get; set; }
    public string Devise { get; set; } = string.Empty;
    public string TauxTVAJson { get; set; } = string.Empty;
    public int DevisValiditeJoursDefaut { get; set; }
    public bool BlocageSiStockInsuffisant { get; set; }
    public string DocumentNumberingFloorsJson { get; set; } = string.Empty;
    public string UiLanguage { get; set; } = string.Empty;
    public bool EnableVirtualKeyboard { get; set; }
    public bool BackupEnabled { get; set; }
    public string BackupDirectory { get; set; } = string.Empty;
    public int BackupIntervalHours { get; set; }
    public string BackupIntervalUnit { get; set; } = string.Empty;
    public int BackupRetentionDays { get; set; }
    public DateTime? LastBackupDate { get; set; }
}
