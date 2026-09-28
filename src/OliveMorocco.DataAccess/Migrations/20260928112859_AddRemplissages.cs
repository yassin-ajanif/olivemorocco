using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRemplissages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ContenanceLitres",
                table: "Produits",
                type: "numeric(8,3)",
                precision: 8,
                scale: 3,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Produits" SET "ContenanceLitres" = 0.5 WHERE "Reference" = 'HVO-500' AND "ContenanceLitres" IS NULL;
                UPDATE "Produits" SET "ContenanceLitres" = 1   WHERE "Reference" = 'HVO-1L'  AND "ContenanceLitres" IS NULL;
                UPDATE "Produits" SET "ContenanceLitres" = 5   WHERE "Reference" = 'HVO-5L'  AND "ContenanceLitres" IS NULL;
                """);

            migrationBuilder.CreateTable(
                name: "Remplissages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    VarieteId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QuantiteHuile = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    Perte = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false, defaultValue: 0m),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remplissages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remplissages_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RemplissageLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RemplissageId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    ContenanceLitres = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    Litres = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemplissageLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemplissageLignes_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RemplissageLignes_Remplissages_RemplissageId",
                        column: x => x.RemplissageId,
                        principalTable: "Remplissages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemplissageLignes_ProduitId",
                table: "RemplissageLignes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_RemplissageLignes_RemplissageId",
                table: "RemplissageLignes",
                column: "RemplissageId");

            migrationBuilder.CreateIndex(
                name: "IX_RemplissageLignes_RemplissageId_ProduitId",
                table: "RemplissageLignes",
                columns: new[] { "RemplissageId", "ProduitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remplissages_Date",
                table: "Remplissages",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Remplissages_Numero",
                table: "Remplissages",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remplissages_VarieteId",
                table: "Remplissages",
                column: "VarieteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemplissageLignes");

            migrationBuilder.DropTable(
                name: "Remplissages");

            migrationBuilder.DropColumn(
                name: "ContenanceLitres",
                table: "Produits");
        }
    }
}
