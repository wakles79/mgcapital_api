using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddSnoozeDateToPreCalendarTaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SnoozeDate",
                table: "WorkOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnoozeDate",
                table: "WorkOrders");
        }
    }
}
