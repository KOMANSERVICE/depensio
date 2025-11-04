using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class anonymebasefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "UsersBoutiques",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UsersBoutiques",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "UsersBoutiques",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UsersBoutiques",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UsersBoutiques",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Subscriptions",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Subscriptions",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Subscriptions",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Subscriptions",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Subscriptions",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "StockLocations",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "StockLocations",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "StockLocations",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "StockLocations",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StockLocations",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Sales",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Sales",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Sales",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Sales",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sales",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "SaleItems",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "SaleItems",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "SaleItems",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SaleItems",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SaleItems",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "PurchaseItem",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PurchaseItem",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PurchaseItem",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PurchaseItem",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PurchaseItem",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Purchase",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Purchase",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Purchase",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Purchase",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Purchase",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Profiles",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Profiles",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Profiles",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Profiles",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Profiles",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ProfileMenus",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ProfileMenus",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ProfileMenus",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProfileMenus",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProfileMenus",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Products",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Products",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Products",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Products",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ProductItems",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ProductItems",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ProductItems",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProductItems",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductItems",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Plans",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Plans",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Plans",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Plans",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Plans",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "PlanMenus",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PlanMenus",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PlanMenus",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PlanMenus",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PlanMenus",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "PlanFeatures",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PlanFeatures",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PlanFeatures",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PlanFeatures",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PlanFeatures",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Menus",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Menus",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Menus",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Menus",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Menus",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Features",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Features",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Features",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Features",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Features",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "BoutiqueSettings",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "BoutiqueSettings",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "BoutiqueSettings",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BoutiqueSettings",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BoutiqueSettings",
                newName: "champ1");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Boutiques",
                newName: "champ5");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Boutiques",
                newName: "champ3");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Boutiques",
                newName: "champ4");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Boutiques",
                newName: "champ2");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Boutiques",
                newName: "champ1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "UsersBoutiques",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "UsersBoutiques",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "UsersBoutiques",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "UsersBoutiques",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "UsersBoutiques",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Subscriptions",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Subscriptions",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Subscriptions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Subscriptions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Subscriptions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "StockLocations",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "StockLocations",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "StockLocations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "StockLocations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "StockLocations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Sales",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Sales",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Sales",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Sales",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Sales",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "SaleItems",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "SaleItems",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "SaleItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "SaleItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "SaleItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "PurchaseItem",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "PurchaseItem",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "PurchaseItem",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "PurchaseItem",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "PurchaseItem",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Purchase",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Purchase",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Purchase",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Purchase",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Purchase",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Profiles",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Profiles",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Profiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Profiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "ProfileMenus",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "ProfileMenus",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "ProfileMenus",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "ProfileMenus",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "ProfileMenus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Products",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Products",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Products",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "ProductItems",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "ProductItems",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "ProductItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "ProductItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "ProductItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Plans",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Plans",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Plans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Plans",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Plans",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "PlanMenus",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "PlanMenus",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "PlanMenus",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "PlanMenus",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "PlanMenus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "PlanFeatures",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "PlanFeatures",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "PlanFeatures",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "PlanFeatures",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "PlanFeatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Menus",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Menus",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Menus",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Menus",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Menus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Features",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Features",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Features",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Features",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Features",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "BoutiqueSettings",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "BoutiqueSettings",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "BoutiqueSettings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "BoutiqueSettings",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "BoutiqueSettings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "champ5",
                table: "Boutiques",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "champ4",
                table: "Boutiques",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "champ3",
                table: "Boutiques",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "champ2",
                table: "Boutiques",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "champ1",
                table: "Boutiques",
                newName: "Id");
        }
    }
}
