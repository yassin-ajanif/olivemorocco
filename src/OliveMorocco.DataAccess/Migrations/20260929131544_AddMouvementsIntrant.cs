using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMouvementsIntrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MouvementsIntrant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntrantId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MouvementsIntrant", x => x.Id);
                    table.CheckConstraint("CK_MouvementsIntrant_Type", "\"Type\" IN ('Entree','Sortie','Ajustement')");
                    table.ForeignKey(
                        name: "FK_MouvementsIntrant_Intrants_IntrantId",
                        column: x => x.IntrantId,
                        principalTable: "Intrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsIntrant_IntrantId",
                table: "MouvementsIntrant",
                column: "IntrantId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsIntrant_OrigineType_OrigineId",
                table: "MouvementsIntrant",
                columns: new[] { "OrigineType", "OrigineId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MouvementsIntrant");
        }
    }
}
