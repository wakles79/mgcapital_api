using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class ExecutedValuesAddedWorkOrderTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "HoursExecuted",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "QuantityExecuted",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursExecuted",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "QuantityExecuted",
                table: "WorkOrderTasks");
        }
    }
}
