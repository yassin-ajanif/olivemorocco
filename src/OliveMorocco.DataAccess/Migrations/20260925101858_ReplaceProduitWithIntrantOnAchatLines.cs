using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OliveMorocco.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceProduitWithIntrantOnAchatLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvoirFournisseurLignes_Produits_ProduitId",
                table: "AvoirFournisseurLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_BonCommandeLignes_Produits_ProduitId",
                table: "BonCommandeLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_BonReceptionLignes_Produits_ProduitId",
                table: "BonReceptionLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_FactureFournisseurLignes_Produits_ProduitId",
                table: "FactureFournisseurLignes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FactureFournisseurLignes_ProduitOrService",
                table: "FactureFournisseurLignes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BonCommandeLignes_ProduitOrService",
                table: "BonCommandeLignes");

            migrationBuilder.RenameColumn(
                name: "ProduitId",
                table: "FactureFournisseurLignes",
                newName: "IntrantId");

            migrationBuilder.RenameIndex(
                name: "IX_FactureFournisseurLignes_ProduitId",
                table: "FactureFournisseurLignes",
                newName: "IX_FactureFournisseurLignes_IntrantId");

            migrationBuilder.RenameColumn(
                name: "ProduitId",
                table: "BonReceptionLignes",
                newName: "IntrantId");

            migrationBuilder.RenameIndex(
                name: "IX_BonReceptionLignes_ProduitId",
                table: "BonReceptionLignes",
                newName: "IX_BonReceptionLignes_IntrantId");

            migrationBuilder.RenameColumn(
                name: "ProduitId",
                table: "BonCommandeLignes",
                newName: "IntrantId");

            migrationBuilder.RenameIndex(
                name: "IX_BonCommandeLignes_ProduitId",
                table: "BonCommandeLignes",
                newName: "IX_BonCommandeLignes_IntrantId");

            migrationBuilder.RenameColumn(
                name: "ProduitId",
                table: "AvoirFournisseurLignes",
                newName: "IntrantId");

            migrationBuilder.RenameIndex(
                name: "IX_AvoirFournisseurLignes_ProduitId",
                table: "AvoirFournisseurLignes",
                newName: "IX_AvoirFournisseurLignes_IntrantId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FactureFournisseurLignes_IntrantOrService",
                table: "FactureFournisseurLignes",
                sql: "(\"IntrantId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"IntrantId\" IS NULL AND \"ServiceId\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BonCommandeLignes_IntrantOrService",
                table: "BonCommandeLignes",
                sql: "(\"IntrantId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"IntrantId\" IS NULL AND \"ServiceId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_AvoirFournisseurLignes_Intrants_IntrantId",
                table: "AvoirFournisseurLignes",
                column: "IntrantId",
                principalTable: "Intrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BonCommandeLignes_Intrants_IntrantId",
                table: "BonCommandeLignes",
                column: "IntrantId",
                principalTable: "Intrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BonReceptionLignes_Intrants_IntrantId",
                table: "BonReceptionLignes",
                column: "IntrantId",
                principalTable: "Intrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FactureFournisseurLignes_Intrants_IntrantId",
                table: "FactureFournisseurLignes",
                column: "IntrantId",
                principalTable: "Intrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvoirFournisseurLignes_Intrants_IntrantId",
                table: "AvoirFournisseurLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_BonCommandeLignes_Intrants_IntrantId",
                table: "BonCommandeLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_BonReceptionLignes_Intrants_IntrantId",
                table: "BonReceptionLignes");

            migrationBuilder.DropForeignKey(
                name: "FK_FactureFournisseurLignes_Intrants_IntrantId",
                table: "FactureFournisseurLignes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FactureFournisseurLignes_IntrantOrService",
                table: "FactureFournisseurLignes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BonCommandeLignes_IntrantOrService",
                table: "BonCommandeLignes");

            migrationBuilder.RenameColumn(
                name: "IntrantId",
                table: "FactureFournisseurLignes",
                newName: "ProduitId");

            migrationBuilder.RenameIndex(
                name: "IX_FactureFournisseurLignes_IntrantId",
                table: "FactureFournisseurLignes",
                newName: "IX_FactureFournisseurLignes_ProduitId");

            migrationBuilder.RenameColumn(
                name: "IntrantId",
                table: "BonReceptionLignes",
                newName: "ProduitId");

            migrationBuilder.RenameIndex(
                name: "IX_BonReceptionLignes_IntrantId",
                table: "BonReceptionLignes",
                newName: "IX_BonReceptionLignes_ProduitId");

            migrationBuilder.RenameColumn(
                name: "IntrantId",
                table: "BonCommandeLignes",
                newName: "ProduitId");

            migrationBuilder.RenameIndex(
                name: "IX_BonCommandeLignes_IntrantId",
                table: "BonCommandeLignes",
                newName: "IX_BonCommandeLignes_ProduitId");

            migrationBuilder.RenameColumn(
                name: "IntrantId",
                table: "AvoirFournisseurLignes",
                newName: "ProduitId");

            migrationBuilder.RenameIndex(
                name: "IX_AvoirFournisseurLignes_IntrantId",
                table: "AvoirFournisseurLignes",
                newName: "IX_AvoirFournisseurLignes_ProduitId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FactureFournisseurLignes_ProduitOrService",
                table: "FactureFournisseurLignes",
                sql: "(\"ProduitId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"ProduitId\" IS NULL AND \"ServiceId\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BonCommandeLignes_ProduitOrService",
                table: "BonCommandeLignes",
                sql: "(\"ProduitId\" IS NOT NULL AND \"ServiceId\" IS NULL) OR (\"ProduitId\" IS NULL AND \"ServiceId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_AvoirFournisseurLignes_Produits_ProduitId",
                table: "AvoirFournisseurLignes",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BonCommandeLignes_Produits_ProduitId",
                table: "BonCommandeLignes",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BonReceptionLignes_Produits_ProduitId",
                table: "BonReceptionLignes",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FactureFournisseurLignes_Produits_ProduitId",
                table: "FactureFournisseurLignes",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
