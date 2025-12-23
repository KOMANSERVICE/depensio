using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleTreasuryIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CashFlowId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Sales",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SaleStatusHistories",
                columns: table => new
                {
                    champ1 = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SaleStatusHistories", x => x.champ1);
                    table.ForeignKey(
                        name: "FK_SaleStatusHistories_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "champ1",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleStatusHistories_SaleId",
                table: "SaleStatusHistories",
                column: "SaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleStatusHistories");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CashFlowId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Sales");
        }
    }
}
