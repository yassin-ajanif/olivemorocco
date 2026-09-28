using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroToInterventionsAndPressages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Pressages",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Interventions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            // Backfill existing rows with unique placeholder values
            migrationBuilder.Sql("""
                UPDATE "Pressages" SET "Numero" = 'PRS-' || "Id" WHERE "Numero" = '';
                UPDATE "Interventions" SET "Numero" = 'INT-' || "Id" WHERE "Numero" = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_Numero",
                table: "Pressages",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_Numero",
                table: "Interventions",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pressages_Numero",
                table: "Pressages");

            migrationBuilder.DropIndex(
                name: "IX_Interventions_Numero",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Pressages");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Interventions");
        }
    }
}
