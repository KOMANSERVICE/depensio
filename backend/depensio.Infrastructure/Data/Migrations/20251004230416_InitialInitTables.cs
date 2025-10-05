using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialInitTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Menus",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // === Features ===
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Key", "CreatedBy", "UpdatedBy", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1a5e3d2-4b71-41f2-9a8b-1a63f3c58a91"), "MAX_USERS", "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("b32e1c89-5c56-4f83-8c9e-2981e23a78f2"), "MAX_PRODUCTS", "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("d90b6f0a-b2b2-4d25-b72b-64876e8c77ab"), "ADVANCED_REPORTS", "seed", "seed", DateTime.UtcNow, DateTime.UtcNow }
                });

            // === Plans ===
            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "Name", "Description", "Price", "RequiresPayment", "IsDisplay", "IsPopular", "CreatedBy", "UpdatedBy", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("9d8f5f7a-032f-456b-8c4a-90b3b6a4d001"), "Gratuit", "Plan gratuit", 0m, false, true, false, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("6c17ff9b-5e13-43c8-9c3b-d8e3b14cb502"), "Débutant", "Plan premium", 10000m, true, true, true, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("2b716a4c-7981-4b08-bc87-8a8d1f0a0503"), "Standard", "Plan entreprise", 15000m, true, true, false, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("1f2d40c5-7c85-44c9-bf93-2f123b2b0404"), "Professionnel", "Plan entreprise", 20000m, true, true, false, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow }
                });

            // === Menus ===
            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Name", "ApiRoute", "UrlFront", "Icon", "Order", "CreatedBy", "UpdatedBy", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1f3a9a11-47f7-4b2c-a4ef-13ab3a11a001"), "Tableau de bord", "/sale", "/dashboard", "fa-solid fa-chart-pie", 1, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("2f4b9b22-58f8-4c3d-b5df-24bc4b22b002"), "Produit", "/product", "/produit", "fa-solid fa-bag-shopping", 2, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("3f5c9c33-69f9-4d4e-c6ef-35cd5c33c003"), "Acheter", "/purchase", "/achatproduit", "fa-solid fa-money-bills", 3, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("4f6d9d44-7afa-4e5f-d7ff-46de6d44d004"), "Caisse", "/sale", "/caisse", "fa-solid fa-cash-register", 4, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("5f7e9e55-8bfb-4f6f-e80f-57ef7e55e005"), "Imprimer code barre", "/product", "/print-barcodes", "fa-solid fa-barcode", 5, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("6f8f9f66-9cfc-507f-f91f-68ff8f66f006"), "Liste utilisateurs", "/purchase", "/liste-user", "fa-solid fa-address-card", 6, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("7f90a077-ad0d-518f-0a2f-7900a077a007"), "Liste des profile", "/profile", "/profile", "fa-solid fa-chalkboard-user", 7, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow },
                    { new Guid("8fa1b188-be1e-529f-1b3f-8a11b188b008"), "Paramètre", "/settings", "/settings", "fa-solid fa-gear", 8, "seed", "seed", DateTime.UtcNow, DateTime.UtcNow }

                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Menus");

            migrationBuilder.Sql("DELETE FROM [PlanMenus]");
            migrationBuilder.Sql("DELETE FROM [PlanFeatures]");
            migrationBuilder.Sql("DELETE FROM [Menus]");
            migrationBuilder.Sql("DELETE FROM [Plans]");
            migrationBuilder.Sql("DELETE FROM [Features]");
        }
    }
}
