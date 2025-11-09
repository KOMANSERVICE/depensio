using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class BoutiqueRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoutiqueSettings_Boutiques_BoutiqueId",
                table: "BoutiqueSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Boutiques_BoutiqueId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Boutiques_BoutiqueId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Boutiques_BoutiqueId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Boutiques_BoutiqueId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLocations_Boutiques_BoutiqueId",
                table: "StockLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Boutiques_BoutiqueId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Boutiques",
                table: "Boutiques");

            migrationBuilder.RenameTable(
                name: "Boutiques",
                newName: "TB00001");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB00001",
                table: "TB00001",
                column: "champ1");

            migrationBuilder.AddForeignKey(
                name: "FK_BoutiqueSettings_TB00001_BoutiqueId",
                table: "BoutiqueSettings",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TB00001_BoutiqueId",
                table: "Products",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_TB00001_BoutiqueId",
                table: "Profiles",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_TB00001_BoutiqueId",
                table: "Purchase",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_TB00001_BoutiqueId",
                table: "Sales",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLocations_TB00001_BoutiqueId",
                table: "StockLocations",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_TB00001_BoutiqueId",
                table: "Subscriptions",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBoutiques_TB00001_BoutiqueId",
                table: "UsersBoutiques",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoutiqueSettings_TB00001_BoutiqueId",
                table: "BoutiqueSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_TB00001_BoutiqueId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_TB00001_BoutiqueId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_TB00001_BoutiqueId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_TB00001_BoutiqueId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLocations_TB00001_BoutiqueId",
                table: "StockLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_TB00001_BoutiqueId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersBoutiques_TB00001_BoutiqueId",
                table: "UsersBoutiques");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB00001",
                table: "TB00001");

            migrationBuilder.RenameTable(
                name: "TB00001",
                newName: "Boutiques");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Boutiques",
                table: "Boutiques",
                column: "champ1");

            migrationBuilder.AddForeignKey(
                name: "FK_BoutiqueSettings_Boutiques_BoutiqueId",
                table: "BoutiqueSettings",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Boutiques_BoutiqueId",
                table: "Products",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Boutiques_BoutiqueId",
                table: "Profiles",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Boutiques_BoutiqueId",
                table: "Purchase",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Boutiques_BoutiqueId",
                table: "Sales",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLocations_Boutiques_BoutiqueId",
                table: "StockLocations",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Boutiques_BoutiqueId",
                table: "Subscriptions",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBoutiques_Boutiques_BoutiqueId",
                table: "UsersBoutiques",
                column: "BoutiqueId",
                principalTable: "Boutiques",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
