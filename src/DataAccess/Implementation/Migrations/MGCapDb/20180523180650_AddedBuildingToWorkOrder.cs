using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedBuildingToWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "WorkOrders",
                newName: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_BuildingId",
                table: "WorkOrders",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Buildings_BuildingId",
                table: "WorkOrders",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Buildings_BuildingId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_BuildingId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "BuildingId",
                table: "WorkOrders",
                newName: "LocationId");
        }
    }
}
