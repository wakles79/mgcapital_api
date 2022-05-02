using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RequesterId_to_nulleable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "RequesterId",
                table: "WorkOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders",
                column: "RequesterId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "RequesterId",
                table: "WorkOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Contacts_RequesterId",
                table: "WorkOrders",
                column: "RequesterId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
