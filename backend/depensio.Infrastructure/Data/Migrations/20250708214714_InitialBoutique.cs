using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialBoutique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques");

            // Étape 1 : ajouter colonne temporaire de type UUID
            migrationBuilder.AddColumn<Guid>(
                name: "UserId_tmp",
                table: "UsersBoutiques",
                type: "uuid",
                nullable: true); // temporairement nullable pour permettre la migration

            // Étape 2 : copier les données converties (attention aux données invalides !)
            migrationBuilder.Sql(@"
        UPDATE ""UsersBoutiques""
        SET ""UserId_tmp"" = ""UserId""::uuid;
    ");

            // Étape 3 : supprimer l’ancienne colonne
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersBoutiques");

            // Étape 4 : renommer la colonne temporaire
            migrationBuilder.RenameColumn(
                name: "UserId_tmp",
                table: "UsersBoutiques",
                newName: "UserId");

            // Étape 5 : rendre non nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersBoutiques",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques");

            // Étape 1 : ajouter colonne temporaire
            migrationBuilder.AddColumn<string>(
                name: "UserId_tmp",
                table: "UsersBoutiques",
                type: "text",
                nullable: true);

            // Étape 2 : convertir vers string
            migrationBuilder.Sql(@"
        UPDATE ""UsersBoutiques""
        SET ""UserId_tmp"" = ""UserId""::text;
    ");

            // Étape 3 : supprimer UserId UUID
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersBoutiques");

            // Étape 4 : renommer
            migrationBuilder.RenameColumn(
                name: "UserId_tmp",
                table: "UsersBoutiques",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersBoutiques",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

    }
}
