using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreditModePaiement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Paiements\" WHERE \"Mode\" = 'Credit';");
            migrationBuilder.Sql("DELETE FROM \"PaiementsFournisseurs\" WHERE \"Mode\" = 'Credit';");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PaiementsFournisseurs_Mode",
                table: "PaiementsFournisseurs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Paiements_Mode",
                table: "Paiements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PaiementsFournisseurs_Mode",
                table: "PaiementsFournisseurs",
                sql: "\"Mode\" IN ('Cheque','Especes','TPE','Virement','Effet')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Paiements_Mode",
                table: "Paiements",
                sql: "\"Mode\" IN ('Cheque','Especes','TPE','Virement','Effet')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PaiementsFournisseurs_Mode",
                table: "PaiementsFournisseurs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Paiements_Mode",
                table: "Paiements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PaiementsFournisseurs_Mode",
                table: "PaiementsFournisseurs",
                sql: "\"Mode\" IN ('Credit','Cheque','Especes','TPE','Virement','Effet')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Paiements_Mode",
                table: "Paiements",
                sql: "\"Mode\" IN ('Credit','Cheque','Especes','TPE','Virement','Effet')");
        }
    }
}
