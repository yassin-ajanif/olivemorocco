using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFactureFournisseurFromPressage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages");

            migrationBuilder.AddForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages",
                column: "FactureFournisseurId",
                principalTable: "FacturesFournisseurs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages");

            migrationBuilder.AddForeignKey(
                name: "FK_Pressages_FacturesFournisseurs_FactureFournisseurId",
                table: "Pressages",
                column: "FactureFournisseurId",
                principalTable: "FacturesFournisseurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
