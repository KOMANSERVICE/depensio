using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_TB00001_BoutiqueId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Products_ProductId",
                table: "PurchaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Purchase_PurchaseId",
                table: "PurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItem",
                table: "PurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase");

            migrationBuilder.RenameTable(
                name: "PurchaseItem",
                newName: "PurchaseItems");

            migrationBuilder.RenameTable(
                name: "Purchase",
                newName: "Purchases");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItem_PurchaseId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItem_ProductId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_BoutiqueId",
                table: "Purchases",
                newName: "IX_Purchases_BoutiqueId");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Purchases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Purchases",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CashFlowId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Purchases",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Purchases",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems",
                column: "champ1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "champ1");

            migrationBuilder.CreateTable(
                name: "PurchaseStatusHistories",
                columns: table => new
                {
                    champ1 = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStatus = table.Column<int>(type: "integer", nullable: true),
                    ToStatus = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    champ2 = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    champ3 = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    champ4 = table.Column<string>(type: "text", nullable: false),
                    champ5 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseStatusHistories", x => x.champ1);
                    table.ForeignKey(
                        name: "FK_PurchaseStatusHistories_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "champ1",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStatusHistories_PurchaseId",
                table: "PurchaseStatusHistories",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Products_ProductId",
                table: "PurchaseItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_TB00001_BoutiqueId",
                table: "Purchases",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Products_ProductId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_TB00001_BoutiqueId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "PurchaseStatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CashFlowId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchase");

            migrationBuilder.RenameTable(
                name: "PurchaseItems",
                newName: "PurchaseItem");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_BoutiqueId",
                table: "Purchase",
                newName: "IX_Purchase_BoutiqueId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_PurchaseId",
                table: "PurchaseItem",
                newName: "IX_PurchaseItem_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItem",
                newName: "IX_PurchaseItem_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase",
                column: "champ1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItem",
                table: "PurchaseItem",
                column: "champ1");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_TB00001_BoutiqueId",
                table: "Purchase",
                column: "BoutiqueId",
                principalTable: "TB00001",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Products_ProductId",
                table: "PurchaseItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Purchase_PurchaseId",
                table: "PurchaseItem",
                column: "PurchaseId",
                principalTable: "Purchase",
                principalColumn: "champ1",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
