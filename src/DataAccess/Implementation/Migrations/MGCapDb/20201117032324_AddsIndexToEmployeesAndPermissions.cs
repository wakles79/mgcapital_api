using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddsIndexToEmployeesAndPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name_Module_Type",
                table: "Permissions",
                columns: new[] { "Name", "Module", "Type" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email_CompanyId",
                table: "Employees",
                columns: new[] { "Email", "CompanyId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_Name_Module_Type",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email_CompanyId",
                table: "Employees");
        }
    }
}
