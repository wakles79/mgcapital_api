using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedBillingFieldsToTaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountPercentage",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_ServiceId",
                table: "WorkOrderTasks",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_Services_ServiceId",
                table: "WorkOrderTasks",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_Services_ServiceId",
                table: "WorkOrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderTasks_ServiceId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "WorkOrderTasks");
        }
    }
}
