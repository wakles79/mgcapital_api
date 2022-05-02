using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedBuildingContactPivtoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingContacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingContacts", x => new { x.ContactId, x.BuildingId });
                    table.UniqueConstraint("AK_BuildingContacts_BuildingId_ContactId", x => new { x.BuildingId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_BuildingContacts_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingContacts");
        }
    }
}
