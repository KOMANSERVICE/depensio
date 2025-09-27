using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace depensio.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserboutiqueType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersBoutiques",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Boutiques",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBoutiques_UserId",
                table: "UsersBoutiques",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBoutiques_AspNetUsers_UserId",
                table: "UsersBoutiques",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBoutiques_AspNetUsers_UserId",
                table: "UsersBoutiques");

            migrationBuilder.DropIndex(
                name: "IX_UsersBoutiques_UserId",
                table: "UsersBoutiques");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersBoutiques",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "Boutiques",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
