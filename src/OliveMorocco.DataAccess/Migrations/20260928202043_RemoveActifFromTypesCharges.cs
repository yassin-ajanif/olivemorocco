using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveActifFromTypesCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actif",
                table: "TypesCharges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Actif",
                table: "TypesCharges",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
