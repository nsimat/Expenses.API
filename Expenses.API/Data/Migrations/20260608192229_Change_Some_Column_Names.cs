using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expenses.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Change_Some_Column_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedFullName",
                table: "UserAccounts",
                newName: "Normalized Full Name");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "UserAccounts",
                newName: "Normalized Email");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "UserAccounts",
                newName: "Last Name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "UserAccounts",
                newName: "First Name");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "UserAccounts",
                newName: "Date Of Birth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Normalized Full Name",
                table: "UserAccounts",
                newName: "NormalizedFullName");

            migrationBuilder.RenameColumn(
                name: "Normalized Email",
                table: "UserAccounts",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "Last Name",
                table: "UserAccounts",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "First Name",
                table: "UserAccounts",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Date Of Birth",
                table: "UserAccounts",
                newName: "DateOfBirth");
        }
    }
}
