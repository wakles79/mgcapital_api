using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class ChangePreCalendarEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendars_Buildings_BuildingId",
                table: "PreCalendars");

            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendars_Companies_CompanyId",
                table: "PreCalendars");

            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendars_Employees_CustomerId",
                table: "PreCalendars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreCalendars",
                table: "PreCalendars");

            migrationBuilder.DropIndex(
                name: "IX_PreCalendars_CustomerId",
                table: "PreCalendars");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PreCalendars");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "PreCalendars");

            migrationBuilder.RenameTable(
                name: "PreCalendars",
                newName: "PreCalendar");

            migrationBuilder.RenameIndex(
                name: "IX_PreCalendars_CompanyId",
                table: "PreCalendar",
                newName: "IX_PreCalendar_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_PreCalendars_BuildingId",
                table: "PreCalendar",
                newName: "IX_PreCalendar_BuildingId");

            migrationBuilder.AddColumn<int>(
                name: "periodicity",
                table: "PreCalendar",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreCalendar",
                table: "PreCalendar",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PreCalendar_EmployeeId",
                table: "PreCalendar",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendar_Buildings_BuildingId",
                table: "PreCalendar",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendar_Companies_CompanyId",
                table: "PreCalendar",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendar_Employees_EmployeeId",
                table: "PreCalendar",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendar_Buildings_BuildingId",
                table: "PreCalendar");

            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendar_Companies_CompanyId",
                table: "PreCalendar");

            migrationBuilder.DropForeignKey(
                name: "FK_PreCalendar_Employees_EmployeeId",
                table: "PreCalendar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreCalendar",
                table: "PreCalendar");

            migrationBuilder.DropIndex(
                name: "IX_PreCalendar_EmployeeId",
                table: "PreCalendar");

            migrationBuilder.DropColumn(
                name: "periodicity",
                table: "PreCalendar");

            migrationBuilder.RenameTable(
                name: "PreCalendar",
                newName: "PreCalendars");

            migrationBuilder.RenameIndex(
                name: "IX_PreCalendar_CompanyId",
                table: "PreCalendars",
                newName: "IX_PreCalendars_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_PreCalendar_BuildingId",
                table: "PreCalendars",
                newName: "IX_PreCalendars_BuildingId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "PreCalendars",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "PreCalendars",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreCalendars",
                table: "PreCalendars",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PreCalendars_CustomerId",
                table: "PreCalendars",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendars_Buildings_BuildingId",
                table: "PreCalendars",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendars_Companies_CompanyId",
                table: "PreCalendars",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreCalendars_Employees_CustomerId",
                table: "PreCalendars",
                column: "CustomerId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
