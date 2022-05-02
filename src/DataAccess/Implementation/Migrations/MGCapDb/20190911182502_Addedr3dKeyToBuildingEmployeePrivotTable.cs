using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class Addedr3dKeyToBuildingEmployeePrivotTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_BuildingEmployees_BuildingId_EmployeeId",
                table: "BuildingEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingEmployees",
                table: "BuildingEmployees");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BuildingEmployees_BuildingId_EmployeeId_Type",
                table: "BuildingEmployees",
                columns: new[] { "BuildingId", "EmployeeId", "Type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingEmployees",
                table: "BuildingEmployees",
                columns: new[] { "EmployeeId", "BuildingId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_BuildingEmployees_BuildingId_EmployeeId_Type",
                table: "BuildingEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingEmployees",
                table: "BuildingEmployees");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BuildingEmployees_BuildingId_EmployeeId",
                table: "BuildingEmployees",
                columns: new[] { "BuildingId", "EmployeeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingEmployees",
                table: "BuildingEmployees",
                columns: new[] { "EmployeeId", "BuildingId" });
        }
    }
}
