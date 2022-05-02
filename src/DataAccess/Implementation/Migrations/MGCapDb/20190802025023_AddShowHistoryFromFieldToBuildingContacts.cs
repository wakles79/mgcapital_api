using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddShowHistoryFromFieldToBuildingContacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShowHistoryFrom",
                table: "BuildingContacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowHistoryFrom",
                table: "BuildingContacts");
        }
    }
}
