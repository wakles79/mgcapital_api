using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MgCapDb
{
    public partial class Removed_WorkOrder_AssignedEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Employees_AssignedEmployeeId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_AssignedEmployeeId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "AssignedEmployeeId",
                table: "WorkOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedEmployeeId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_AssignedEmployeeId",
                table: "WorkOrders",
                column: "AssignedEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Employees_AssignedEmployeeId",
                table: "WorkOrders",
                column: "AssignedEmployeeId",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
