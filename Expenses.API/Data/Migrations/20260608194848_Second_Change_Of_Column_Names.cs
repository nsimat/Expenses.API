using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expenses.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Second_Change_Of_Column_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserAccounts_UserId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserAccounts",
                newName: "Latest Update");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserAccounts",
                newName: "Date of Creation");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Transactions",
                newName: "Creator User ID");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Transactions",
                newName: "Latest Update");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Transactions",
                newName: "Date of Creation");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                newName: "IX_Transactions_Creator User ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserAccounts_Creator User ID",
                table: "Transactions",
                column: "Creator User ID",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_UserAccounts_Creator User ID",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Latest Update",
                table: "UserAccounts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Date of Creation",
                table: "UserAccounts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Latest Update",
                table: "Transactions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Date of Creation",
                table: "Transactions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Creator User ID",
                table: "Transactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_Creator User ID",
                table: "Transactions",
                newName: "IX_Transactions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_UserAccounts_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
