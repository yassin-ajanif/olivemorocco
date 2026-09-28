using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddChargeIdToPressage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChargeId",
                table: "Pressages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PressageId",
                table: "Charges",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pressages_ChargeId",
                table: "Pressages",
                column: "ChargeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pressages_Charges_ChargeId",
                table: "Pressages",
                column: "ChargeId",
                principalTable: "Charges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pressages_Charges_ChargeId",
                table: "Pressages");

            migrationBuilder.DropIndex(
                name: "IX_Pressages_ChargeId",
                table: "Pressages");

            migrationBuilder.DropColumn(
                name: "ChargeId",
                table: "Pressages");

            migrationBuilder.DropColumn(
                name: "PressageId",
                table: "Charges");
        }
    }
}
