using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddDescriptionToPreCalendarEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "periodicity",
                table: "PreCalendar",
                newName: "Periodicity");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PreCalendar",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PreCalendar");

            migrationBuilder.RenameColumn(
                name: "Periodicity",
                table: "PreCalendar",
                newName: "periodicity");
        }
    }
}
