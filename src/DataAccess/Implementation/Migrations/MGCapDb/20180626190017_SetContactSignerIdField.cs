using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class SetContactSignerIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Contacts_SignerContactId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_SignerContactId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignerContactId",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContactSignerId",
                table: "Contracts",
                column: "ContactSignerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Contacts_ContactSignerId",
                table: "Contracts",
                column: "ContactSignerId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Contacts_ContactSignerId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContactSignerId",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "SignerContactId",
                table: "Contracts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SignerContactId",
                table: "Contracts",
                column: "SignerContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Contacts_SignerContactId",
                table: "Contracts",
                column: "SignerContactId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
