using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddFreshdeskCredentialsToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreshdeskAgentId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreshdeskApiKey",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasFreshdeskAccount",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FreshdeskAgentId",
                table: "Employees",
                column: "FreshdeskAgentId",
                unique: true,
                filter: "[FreshdeskAgentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FreshdeskApiKey",
                table: "Employees",
                column: "FreshdeskApiKey",
                unique: true,
                filter: "[FreshdeskApiKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_FreshdeskAgentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_FreshdeskApiKey",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FreshdeskAgentId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FreshdeskApiKey",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HasFreshdeskAccount",
                table: "Employees");
        }
    }
}
