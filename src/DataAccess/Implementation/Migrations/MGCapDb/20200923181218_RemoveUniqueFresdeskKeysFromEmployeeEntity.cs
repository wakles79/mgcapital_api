using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveUniqueFresdeskKeysFromEmployeeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_FreshdeskAgentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_FreshdeskApiKey",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "FreshdeskApiKey",
                table: "Employees",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FreshdeskAgentId",
                table: "Employees",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FreshdeskApiKey",
                table: "Employees",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FreshdeskAgentId",
                table: "Employees",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

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
    }
}
