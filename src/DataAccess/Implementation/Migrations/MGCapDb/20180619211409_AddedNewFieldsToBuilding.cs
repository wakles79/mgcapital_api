using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedNewFieldsToBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Buildings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "OperationsManagerId",
                table: "Buildings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Buildings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsAvailable",
                table: "Buildings",
                nullable: false,
                computedColumnSql: "CASE WHEN [OperationsManagerId] IS NOT NULL AND [IsActive] = 1 THEN 1 ELSE 0 END");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_OperationsManagerId",
                table: "Buildings",
                column: "OperationsManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_SupervisorId",
                table: "Buildings",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Employees_OperationsManagerId",
                table: "Buildings",
                column: "OperationsManagerId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Employees_SupervisorId",
                table: "Buildings",
                column: "SupervisorId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Employees_OperationsManagerId",
                table: "Buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Employees_SupervisorId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_OperationsManagerId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_SupervisorId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "OperationsManagerId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Buildings");
        }
    }
}
