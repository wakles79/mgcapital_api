using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class ChangedStatusColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WOSourceId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "WOSourceId",
                table: "WorkOrders",
                newName: "WorkOrderSourceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_WOSourceId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_WorkOrderSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WorkOrderSourceId",
                table: "WorkOrders",
                column: "WorkOrderSourceId",
                principalTable: "WorkOrderSources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WorkOrderSourceId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "WorkOrderSourceId",
                table: "WorkOrders",
                newName: "WOSourceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_WorkOrderSourceId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_WOSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WOSourceId",
                table: "WorkOrders",
                column: "WOSourceId",
                principalTable: "WorkOrderSources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
