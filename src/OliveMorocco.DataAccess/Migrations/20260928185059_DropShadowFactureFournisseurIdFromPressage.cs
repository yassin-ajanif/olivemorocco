using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DropShadowFactureFournisseurIdFromPressage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages");

            migrationBuilder.DropIndex(
                name: "IX_Pressages_FactureFournisseurId",
                table: "Pressages");

            migrationBuilder.DropColumn(
                name: "FactureFournisseurId",
                table: "Pressages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FactureFournisseurId",
                table: "Pressages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_FactureFournisseurId",
                table: "Pressages",
                column: "FactureFournisseurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages",
                column: "FactureFournisseurId",
                principalTable: "FacturesFournisseurs",
                principalColumn: "Id");
        }
    }
}
