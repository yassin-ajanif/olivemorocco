using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocieteNom = table.Column<string>(type: "text", nullable: false),
                    SocieteAdresse = table.Column<string>(type: "text", nullable: false),
                    SocieteICE = table.Column<string>(type: "text", nullable: false),
                    SocieteLogoPath = table.Column<string>(type: "text", nullable: true),
                    SocieteMentionsLegales = table.Column<string>(type: "text", nullable: true),
                    Devise = table.Column<string>(type: "text", nullable: false),
                    TauxTVAJson = table.Column<string>(type: "text", nullable: false),
                    DevisValiditeJoursDefaut = table.Column<int>(type: "integer", nullable: false),
                    BlocageSiStockInsuffisant = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentNumberingFloorsJson = table.Column<string>(type: "text", nullable: false),
                    UiLanguage = table.Column<string>(type: "text", nullable: false),
                    EnableVirtualKeyboard = table.Column<bool>(type: "boolean", nullable: false),
                    BackupEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    BackupDirectory = table.Column<string>(type: "text", nullable: false),
                    BackupIntervalHours = table.Column<int>(type: "integer", nullable: false),
                    BackupIntervalUnit = table.Column<string>(type: "text", nullable: false),
                    BackupRetentionDays = table.Column<int>(type: "integer", nullable: false),
                    LastBackupDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Intrants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Unite = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Secteurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SuperficieHectares = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secteurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Unite = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PrixAchatHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TauxTVA = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Actif = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Adresse = table.Column<string>(type: "text", nullable: false),
                    Ville = table.Column<string>(type: "text", nullable: false),
                    Telephone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ICE = table.Column<string>(type: "text", nullable: false),
                    ConditionsPaiement = table.Column<string>(type: "text", nullable: false),
                    Actif = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                    table.CheckConstraint("CK_Tiers_Type", "\"Type\" IN ('Client','Fournisseur','LesDeux')");
                });

            migrationBuilder.CreateTable(
                name: "TypesCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Actif = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Varietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    RegionOrigine = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varietes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecteurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IntrantId = table.Column<int>(type: "integer", nullable: true),
                    QuantiteIntrant = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: true),
                    QuantiteEau = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_Intrants_IntrantId",
                        column: x => x.IntrantId,
                        principalTable: "Intrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interventions_Secteurs_SecteurId",
                        column: x => x.SecteurId,
                        principalTable: "Secteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvoirsFournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    FournisseurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Motif = table.Column<string>(type: "text", nullable: false),
                    RetourMarchandise = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvoirsFournisseurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvoirsFournisseurs_Tiers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonsCommande",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    FournisseurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsCommande", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonsCommande_Tiers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateValidite = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemiseGlobale = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devis_Tiers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacturesFournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    FournisseurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEcheance = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemiseGlobale = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalTtc = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EstPayee = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturesFournisseurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturesFournisseurs_Tiers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    VarieteId = table.Column<int>(type: "integer", nullable: true),
                    Unite = table.Column<string>(type: "text", nullable: false),
                    CodeBarre = table.Column<string>(type: "text", nullable: true),
                    PrixAchatHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PrixVenteHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StockActuel = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    StockMinimum = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    Actif = table.Column<bool>(type: "boolean", nullable: false),
                    ImageData = table.Column<byte[]>(type: "bytea", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produits_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recoltes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecteurId = table.Column<int>(type: "integer", nullable: false),
                    VarieteId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recoltes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recoltes_Secteurs_SecteurId",
                        column: x => x.SecteurId,
                        principalTable: "Secteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recoltes_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecteurVarietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecteurId = table.Column<int>(type: "integer", nullable: false),
                    VarieteId = table.Column<int>(type: "integer", nullable: false),
                    SuperficieHectares = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecteurVarietes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecteurVarietes_Secteurs_SecteurId",
                        column: x => x.SecteurId,
                        principalTable: "Secteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecteurVarietes_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Charges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeChargeId = table.Column<int>(type: "integer", nullable: false),
                    InterventionId = table.Column<int>(type: "integer", nullable: true),
                    Libelle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MontantTtc = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charges_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Charges_TypesCharges_TypeChargeId",
                        column: x => x.TypeChargeId,
                        principalTable: "TypesCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Factures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEcheance = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DevisId = table.Column<int>(type: "integer", nullable: true),
                    BonCommandeReference = table.Column<string>(type: "text", nullable: false),
                    RemiseGlobale = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalTtc = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EstPayee = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factures_Devis_DevisId",
                        column: x => x.DevisId,
                        principalTable: "Devis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Factures_Tiers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonsReception",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    FournisseurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BonCommandeId = table.Column<int>(type: "integer", nullable: true),
                    FactureFournisseurId = table.Column<int>(type: "integer", nullable: true),
                    TotalTtc = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsReception", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonsReception_BonsCommande_BonCommandeId",
                        column: x => x.BonCommandeId,
                        principalTable: "BonsCommande",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsReception_FacturesFournisseurs_FactureFournisseurId",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FacturesFournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsReception_Tiers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaiementsFournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactureFournisseurId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Montant = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    EstEncaisse = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaiementsFournisseurs", x => x.Id);
                    table.CheckConstraint("CK_PaiementsFournisseurs_Mode", "\"Mode\" IN ('Credit','Cheque','Especes','TPE','Virement','Effet')");
                    table.ForeignKey(
                        name: "FK_PaiementsFournisseurs_FacturesFournisseurs_FactureFournisse~",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FacturesFournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pressages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FournisseurId = table.Column<int>(type: "integer", nullable: false),
                    VarieteId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QuantiteOlives = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    Rendement = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    QuantiteHuile = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: true),
                    FactureFournisseurId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pressages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FacturesFournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pressages_Tiers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pressages_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvoirFournisseurLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvoirFournisseurId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvoirFournisseurLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvoirFournisseurLignes_AvoirsFournisseurs_AvoirFournisseurId",
                        column: x => x.AvoirFournisseurId,
                        principalTable: "AvoirsFournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvoirFournisseurLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonCommandeLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BonCommandeFournisseurId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: true),
                    ServiceId = table.Column<int>(type: "integer", nullable: true),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    QuantiteCommandee = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonCommandeLignes", x => x.Id);
                    table.CheckConstraint("CK_BonCommandeLignes_ProduitOrService", "(\"ProduitId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"ProduitId\" IS NULL AND \"ServiceId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_BonCommandeLignes_BonsCommande_BonCommandeFournisseurId",
                        column: x => x.BonCommandeFournisseurId,
                        principalTable: "BonsCommande",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonCommandeLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonCommandeLignes_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevisLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DevisClientId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevisLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevisLignes_Devis_DevisClientId",
                        column: x => x.DevisClientId,
                        principalTable: "Devis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DevisLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MouvementsStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    StockAvant = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    OrigineType = table.Column<string>(type: "text", nullable: false),
                    OrigineId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouvementsStock", x => x.Id);
                    table.CheckConstraint("CK_MouvementsStock_Type", "\"Type\" IN ('Entree','Sortie','Ajustement')");
                    table.ForeignKey(
                        name: "FK_MouvementsStock_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avoirs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FactureId = table.Column<int>(type: "integer", nullable: true),
                    Motif = table.Column<string>(type: "text", nullable: false),
                    RetourMarchandise = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avoirs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avoirs_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Avoirs_Tiers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonsCommandeClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DevisId = table.Column<int>(type: "integer", nullable: true),
                    FactureId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsCommandeClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonsCommandeClient_Devis_DevisId",
                        column: x => x.DevisId,
                        principalTable: "Devis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsCommandeClient_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsCommandeClient_Tiers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paiements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactureClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Montant = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    EstEncaisse = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paiements", x => x.Id);
                    table.CheckConstraint("CK_Paiements_Mode", "\"Mode\" IN ('Credit','Cheque','Especes','TPE','Virement','Effet')");
                    table.ForeignKey(
                        name: "FK_Paiements_Factures_FactureClientId",
                        column: x => x.FactureClientId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonReceptionLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BRId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    QuantiteRecue = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonReceptionLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonReceptionLignes_BonsReception_BRId",
                        column: x => x.BRId,
                        principalTable: "BonsReception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonReceptionLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactureFournisseurLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactureFournisseurId = table.Column<int>(type: "integer", nullable: false),
                    BonReceptionId = table.Column<int>(type: "integer", nullable: true),
                    ProduitId = table.Column<int>(type: "integer", nullable: true),
                    ServiceId = table.Column<int>(type: "integer", nullable: true),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureFournisseurLignes", x => x.Id);
                    table.CheckConstraint("CK_FactureFournisseurLignes_ProduitOrService", "(\"ProduitId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"ProduitId\" IS NULL AND \"ServiceId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_FactureFournisseurLignes_BonsReception_BonReceptionId",
                        column: x => x.BonReceptionId,
                        principalTable: "BonsReception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FactureFournisseurLignes_FacturesFournisseurs_FactureFourni~",
                        column: x => x.FactureFournisseurId,
                        principalTable: "FacturesFournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactureFournisseurLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactureFournisseurLignes_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvoirLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvoirClientId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvoirLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvoirLignes_Avoirs_AvoirClientId",
                        column: x => x.AvoirClientId,
                        principalTable: "Avoirs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvoirLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonCommandeClientLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BonCommandeClientId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    QuantiteCommandee = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonCommandeClientLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonCommandeClientLignes_BonsCommandeClient_BonCommandeClien~",
                        column: x => x.BonCommandeClientId,
                        principalTable: "BonsCommandeClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonCommandeClientLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonsLivraison",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DevisId = table.Column<int>(type: "integer", nullable: true),
                    BonCommandeClientId = table.Column<int>(type: "integer", nullable: true),
                    FactureId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsLivraison", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonsLivraison_BonsCommandeClient_BonCommandeClientId",
                        column: x => x.BonCommandeClientId,
                        principalTable: "BonsCommandeClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsLivraison_Devis_DevisId",
                        column: x => x.DevisId,
                        principalTable: "Devis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsLivraison_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BonsLivraison_Tiers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BonLivraisonLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BLId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    QuantiteCommandee = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    QuantiteLivree = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonLivraisonLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonLivraisonLignes_BonsLivraison_BLId",
                        column: x => x.BLId,
                        principalTable: "BonsLivraison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonLivraisonLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactureLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactureClientId = table.Column<int>(type: "integer", nullable: false),
                    BonLivraisonId = table.Column<int>(type: "integer", nullable: true),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    Conditionnement = table.Column<string>(type: "text", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Remise = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TauxTVA = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactureLignes_BonsLivraison_BonLivraisonId",
                        column: x => x.BonLivraisonId,
                        principalTable: "BonsLivraison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FactureLignes_Factures_FactureClientId",
                        column: x => x.FactureClientId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactureLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseurLignes_AvoirFournisseurId",
                table: "AvoirFournisseurLignes",
                column: "AvoirFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirFournisseurLignes_ProduitId",
                table: "AvoirFournisseurLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirLignes_AvoirClientId",
                table: "AvoirLignes",
                column: "AvoirClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirLignes_ProduitId",
                table: "AvoirLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_ClientId",
                table: "Avoirs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_FactureId",
                table: "Avoirs",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_Avoirs_Numero",
                table: "Avoirs",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvoirsFournisseurs_FournisseurId",
                table: "AvoirsFournisseurs",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_AvoirsFournisseurs_Numero",
                table: "AvoirsFournisseurs",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonCommandeClientLignes_BonCommandeClientId",
                table: "BonCommandeClientLignes",
                column: "BonCommandeClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonCommandeClientLignes_ProduitId",
                table: "BonCommandeClientLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_BonCommandeLignes_BonCommandeFournisseurId",
                table: "BonCommandeLignes",
                column: "BonCommandeFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_BonCommandeLignes_ProduitId",
                table: "BonCommandeLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_BonCommandeLignes_ServiceId",
                table: "BonCommandeLignes",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_BonLivraisonLignes_BLId",
                table: "BonLivraisonLignes",
                column: "BLId");

            migrationBuilder.CreateIndex(
                name: "IX_BonLivraisonLignes_ProduitId",
                table: "BonLivraisonLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_BonReceptionLignes_BRId",
                table: "BonReceptionLignes",
                column: "BRId");

            migrationBuilder.CreateIndex(
                name: "IX_BonReceptionLignes_ProduitId",
                table: "BonReceptionLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommande_FournisseurId",
                table: "BonsCommande",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommande_Numero",
                table: "BonsCommande",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommandeClient_ClientId",
                table: "BonsCommandeClient",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommandeClient_DevisId",
                table: "BonsCommandeClient",
                column: "DevisId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommandeClient_FactureId",
                table: "BonsCommandeClient",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsCommandeClient_Numero",
                table: "BonsCommandeClient",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsLivraison_BonCommandeClientId",
                table: "BonsLivraison",
                column: "BonCommandeClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsLivraison_ClientId",
                table: "BonsLivraison",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsLivraison_DevisId",
                table: "BonsLivraison",
                column: "DevisId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsLivraison_FactureId",
                table: "BonsLivraison",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsLivraison_Numero",
                table: "BonsLivraison",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsReception_BonCommandeId",
                table: "BonsReception",
                column: "BonCommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsReception_FactureFournisseurId",
                table: "BonsReception",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsReception_FournisseurId",
                table: "BonsReception",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsReception_Numero",
                table: "BonsReception",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Charges_Date",
                table: "Charges",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_InterventionId",
                table: "Charges",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_TypeChargeId",
                table: "Charges",
                column: "TypeChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_Devis_ClientId",
                table: "Devis",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Devis_Numero",
                table: "Devis",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DevisLignes_DevisClientId",
                table: "DevisLignes",
                column: "DevisClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DevisLignes_ProduitId",
                table: "DevisLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseurLignes_BonReceptionId",
                table: "FactureFournisseurLignes",
                column: "BonReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseurLignes_FactureFournisseurId",
                table: "FactureFournisseurLignes",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseurLignes_ProduitId",
                table: "FactureFournisseurLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureFournisseurLignes_ServiceId",
                table: "FactureFournisseurLignes",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureLignes_BonLivraisonId",
                table: "FactureLignes",
                column: "BonLivraisonId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureLignes_FactureClientId",
                table: "FactureLignes",
                column: "FactureClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureLignes_ProduitId",
                table: "FactureLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_ClientId",
                table: "Factures",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_DevisId",
                table: "Factures",
                column: "DevisId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_Numero",
                table: "Factures",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturesFournisseurs_FournisseurId",
                table: "FacturesFournisseurs",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturesFournisseurs_Numero",
                table: "FacturesFournisseurs",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_Date",
                table: "Interventions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_IntrantId",
                table: "Interventions",
                column: "IntrantId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_SecteurId",
                table: "Interventions",
                column: "SecteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Intrants_Nom",
                table: "Intrants",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsStock_ProduitId",
                table: "MouvementsStock",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Paiements_FactureClientId",
                table: "Paiements",
                column: "FactureClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PaiementsFournisseurs_FactureFournisseurId",
                table: "PaiementsFournisseurs",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_Date",
                table: "Pressages",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_FactureFournisseurId",
                table: "Pressages",
                column: "FactureFournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_FournisseurId",
                table: "Pressages",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_VarieteId",
                table: "Pressages",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_Reference",
                table: "Produits",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produits_VarieteId",
                table: "Produits",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Recoltes_Date",
                table: "Recoltes",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Recoltes_SecteurId",
                table: "Recoltes",
                column: "SecteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Recoltes_VarieteId",
                table: "Recoltes",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Secteurs_Code",
                table: "Secteurs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Secteurs_Nom",
                table: "Secteurs",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecteurVarietes_SecteurId_VarieteId",
                table: "SecteurVarietes",
                columns: new[] { "SecteurId", "VarieteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecteurVarietes_VarieteId",
                table: "SecteurVarietes",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Nom",
                table: "Services",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_Reference",
                table: "Services",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesCharges_Nom",
                table: "TypesCharges",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varietes_Code",
                table: "Varietes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varietes_Nom",
                table: "Varietes",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AvoirFournisseurLignes");

            migrationBuilder.DropTable(
                name: "AvoirLignes");

            migrationBuilder.DropTable(
                name: "BonCommandeClientLignes");

            migrationBuilder.DropTable(
                name: "BonCommandeLignes");

            migrationBuilder.DropTable(
                name: "BonLivraisonLignes");

            migrationBuilder.DropTable(
                name: "BonReceptionLignes");

            migrationBuilder.DropTable(
                name: "Charges");

            migrationBuilder.DropTable(
                name: "DevisLignes");

            migrationBuilder.DropTable(
                name: "FactureFournisseurLignes");

            migrationBuilder.DropTable(
                name: "FactureLignes");

            migrationBuilder.DropTable(
                name: "MouvementsStock");

            migrationBuilder.DropTable(
                name: "Paiements");

            migrationBuilder.DropTable(
                name: "PaiementsFournisseurs");

            migrationBuilder.DropTable(
                name: "Pressages");

            migrationBuilder.DropTable(
                name: "Recoltes");

            migrationBuilder.DropTable(
                name: "SecteurVarietes");

            migrationBuilder.DropTable(
                name: "AvoirsFournisseurs");

            migrationBuilder.DropTable(
                name: "Avoirs");

            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "TypesCharges");

            migrationBuilder.DropTable(
                name: "BonsReception");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "BonsLivraison");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Intrants");

            migrationBuilder.DropTable(
                name: "Secteurs");

            migrationBuilder.DropTable(
                name: "BonsCommande");

            migrationBuilder.DropTable(
                name: "FacturesFournisseurs");

            migrationBuilder.DropTable(
                name: "BonsCommandeClient");

            migrationBuilder.DropTable(
                name: "Varietes");

            migrationBuilder.DropTable(
                name: "Factures");

            migrationBuilder.DropTable(
                name: "Devis");

            migrationBuilder.DropTable(
                name: "Tiers");
        }
    }
}
