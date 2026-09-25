using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddInterventionLignes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterventionLignes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterventionId = table.Column<int>(type: "integer", nullable: false),
                    IntrantId = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionLignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionLignes_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionLignes_Intrants_IntrantId",
                        column: x => x.IntrantId,
                        principalTable: "Intrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionLignes_InterventionId",
                table: "InterventionLignes",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionLignes_InterventionId_IntrantId",
                table: "InterventionLignes",
                columns: new[] { "InterventionId", "IntrantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterventionLignes_IntrantId",
                table: "InterventionLignes",
                column: "IntrantId");

            migrationBuilder.Sql("""
                INSERT INTO "InterventionLignes" ("InterventionId", "IntrantId", "Quantite", "CreatedAt", "UpdatedAt", "CreatedByUserId")
                SELECT "Id", "IntrantId", COALESCE("QuantiteIntrant", 0), "CreatedAt", "UpdatedAt", "CreatedByUserId"
                FROM "Interventions"
                WHERE "IntrantId" IS NOT NULL;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Interventions_Intrants_IntrantId",
                table: "Interventions");

            migrationBuilder.DropIndex(
                name: "IX_Interventions_IntrantId",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "IntrantId",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "QuantiteIntrant",
                table: "Interventions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntrantId",
                table: "Interventions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantiteIntrant",
                table: "Interventions",
                type: "numeric(12,4)",
                precision: 12,
                scale: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_IntrantId",
                table: "Interventions",
                column: "IntrantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interventions_Intrants_IntrantId",
                table: "Interventions",
                column: "IntrantId",
                principalTable: "Intrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                UPDATE "Interventions" i
                SET "IntrantId" = l."IntrantId",
                    "QuantiteIntrant" = l."Quantite"
                FROM (
                    SELECT DISTINCT ON ("InterventionId")
                        "InterventionId", "IntrantId", "Quantite"
                    FROM "InterventionLignes"
                    ORDER BY "InterventionId", "Id"
                ) l
                WHERE i."Id" = l."InterventionId";
                """);

            migrationBuilder.DropTable(
                name: "InterventionLignes");
        }
    }
}
