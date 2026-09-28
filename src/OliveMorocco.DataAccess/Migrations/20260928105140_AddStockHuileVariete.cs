using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStockHuileVariete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StockHuile",
                table: "Varietes",
                type: "numeric(12,4)",
                precision: 12,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MouvementsStockVariete",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VarieteId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    StockAvant = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    OrigineType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    OrigineId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouvementsStockVariete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouvementsStockVariete_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsStockVariete_OrigineType_OrigineId",
                table: "MouvementsStockVariete",
                columns: new[] { "OrigineType", "OrigineId" });

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsStockVariete_VarieteId",
                table: "MouvementsStockVariete",
                column: "VarieteId");

            // Type 0 = TypeMouvement.Entree; StockAvant is the running total per variety in pressage order.
            migrationBuilder.Sql("""
                INSERT INTO "MouvementsStockVariete"
                    ("VarieteId", "Type", "Quantite", "StockAvant", "OrigineType", "OrigineId", "Note", "CreatedAt", "UpdatedAt")
                SELECT
                    p."VarieteId",
                    0,
                    p."QuantiteHuile",
                    COALESCE(SUM(p."QuantiteHuile") OVER (
                        PARTITION BY p."VarieteId"
                        ORDER BY p."Date", p."Id"
                        ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING), 0),
                    'Pressage',
                    p."Id",
                    '',
                    NOW(),
                    NOW()
                FROM "Pressages" p
                WHERE p."QuantiteHuile" IS NOT NULL AND p."QuantiteHuile" > 0;

                UPDATE "Varietes" v
                SET "StockHuile" = s.total
                FROM (
                    SELECT "VarieteId", SUM("QuantiteHuile") AS total
                    FROM "Pressages"
                    WHERE "QuantiteHuile" IS NOT NULL AND "QuantiteHuile" > 0
                    GROUP BY "VarieteId"
                ) s
                WHERE v."Id" = s."VarieteId";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MouvementsStockVariete");

            migrationBuilder.DropColumn(
                name: "StockHuile",
                table: "Varietes");
        }
    }
}
