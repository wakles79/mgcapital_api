using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RenamedWOSourcesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_WorkOrderSource_WOSourceId",
                table: "WorkOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderSource",
                table: "WorkOrderSource");

            migrationBuilder.RenameTable(
                name: "WorkOrderSource",
                newName: "WorkOrderSources");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderSources",
                table: "WorkOrderSources",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WOSourceId",
                table: "WorkOrders",
                column: "WOSourceId",
                principalTable: "WorkOrderSources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_WorkOrderSources_WOSourceId",
                table: "WorkOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderSources",
                table: "WorkOrderSources");

            migrationBuilder.RenameTable(
                name: "WorkOrderSources",
                newName: "WorkOrderSource");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderSource",
                table: "WorkOrderSource",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_WorkOrderSource_WOSourceId",
                table: "WorkOrders",
                column: "WOSourceId",
                principalTable: "WorkOrderSource",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
