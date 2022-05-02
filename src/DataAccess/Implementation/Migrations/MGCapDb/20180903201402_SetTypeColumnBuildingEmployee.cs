using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MgCapDb
{
    public partial class SetTypeColumnBuildingEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Employees_SupervisorId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_SupervisorId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Buildings");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "BuildingEmployees",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 80,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Buildings",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "BuildingEmployees",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_SupervisorId",
                table: "Buildings",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Employees_SupervisorId",
                table: "Buildings",
                column: "SupervisorId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
