using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class Addedr3dKeyToWorkOrderEmployeePrivotTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderEmployees",
                table: "WorkOrderEmployees");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WorkOrderEmployees_EmployeeId_Type_WorkOrderId",
                table: "WorkOrderEmployees",
                columns: new[] { "EmployeeId", "WorkOrderId", "Type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderEmployees",
                table: "WorkOrderEmployees",
                columns: new[] { "EmployeeId", "WorkOrderId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WorkOrderEmployees_EmployeeId_Type_WorkOrderId",
                table: "WorkOrderEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderEmployees",
                table: "WorkOrderEmployees");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderEmployees",
                table: "WorkOrderEmployees",
                columns: new[] { "EmployeeId", "WorkOrderId" });
        }
    }
}
