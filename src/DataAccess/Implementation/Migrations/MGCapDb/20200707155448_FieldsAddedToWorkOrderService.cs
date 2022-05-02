using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class FieldsAddedToWorkOrderService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HoursRequiredAtClose",
                table: "WorkOrderServices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "QuantityRequiredAtClose",
                table: "WorkOrderServices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresScheduling",
                table: "WorkOrderServices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursRequiredAtClose",
                table: "WorkOrderServices");

            migrationBuilder.DropColumn(
                name: "QuantityRequiredAtClose",
                table: "WorkOrderServices");

            migrationBuilder.DropColumn(
                name: "RequiresScheduling",
                table: "WorkOrderServices");
        }
    }
}
