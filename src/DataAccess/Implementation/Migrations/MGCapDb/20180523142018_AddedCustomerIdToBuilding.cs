using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedCustomerIdToBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Buildings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CustomerId",
                table: "Buildings",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Customers_CustomerId",
                table: "Buildings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Customers_CustomerId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_CustomerId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Buildings");
        }
    }
}
