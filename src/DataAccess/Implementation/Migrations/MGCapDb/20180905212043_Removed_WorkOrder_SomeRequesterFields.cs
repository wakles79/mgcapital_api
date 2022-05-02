using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MgCapDb
{
    public partial class Removed_WorkOrder_SomeRequesterFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_RequesterId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterExt",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterFirstName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterLastName",
                table: "WorkOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequesterExt",
                table: "WorkOrders",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterFirstName",
                table: "WorkOrders",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequesterId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterLastName",
                table: "WorkOrders",
                maxLength: 80,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_RequesterId",
                table: "WorkOrders",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders",
                column: "RequesterId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
