using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RequireProduitVarieteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produits_Varietes_VarieteId",
                table: "Produits");

            migrationBuilder.Sql("""
                UPDATE "Produits"
                SET "VarieteId" = (
                    SELECT "Id" FROM "Varietes" ORDER BY "Id" LIMIT 1
                )
                WHERE "VarieteId" IS NULL
                  AND EXISTS (SELECT 1 FROM "Varietes");
                """);

            migrationBuilder.AlterColumn<int>(
                name: "VarieteId",
                table: "Produits",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_Varietes_VarieteId",
                table: "Produits",
                column: "VarieteId",
                principalTable: "Varietes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produits_Varietes_VarieteId",
                table: "Produits");

            migrationBuilder.AlterColumn<int>(
                name: "VarieteId",
                table: "Produits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_Varietes_VarieteId",
                table: "Produits",
                column: "VarieteId",
                principalTable: "Varietes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
