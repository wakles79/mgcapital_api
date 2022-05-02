using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveOperationManagerAndComputedColumnIsAvailableForContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Employees_OperationsManagerId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_OperationsManagerId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsAvailableForContract",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "OperationsManagerId",
                table: "Buildings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationsManagerId",
                table: "Buildings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsAvailableForContract",
                table: "Buildings",
                nullable: false,
                computedColumnSql: "CASE WHEN [OperationsManagerId] IS NOT NULL AND [IsActive] = 1 THEN 1 ELSE 0 END");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_OperationsManagerId",
                table: "Buildings",
                column: "OperationsManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Employees_OperationsManagerId",
                table: "Buildings",
                column: "OperationsManagerId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
