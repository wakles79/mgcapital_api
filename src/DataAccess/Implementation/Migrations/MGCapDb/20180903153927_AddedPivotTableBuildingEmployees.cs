using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MgCapDb
{
    public partial class AddedPivotTableBuildingEmployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingEmployees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingEmployees", x => new { x.EmployeeId, x.BuildingId });
                    table.UniqueConstraint("AK_BuildingEmployees_BuildingId_EmployeeId", x => new { x.BuildingId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_BuildingEmployees_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BuildingEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingEmployees");
        }
    }
}
