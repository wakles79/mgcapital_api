using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedCleaningReportItemEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleaningReportItems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuildingId = table.Column<int>(nullable: false),
                    CleaningReportId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Location = table.Column<string>(maxLength: 80, nullable: true),
                    Observances = table.Column<string>(nullable: true),
                    Time = table.Column<string>(maxLength: 16, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningReportItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CleaningReportItems_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CleaningReportItems_CleaningReports_CleaningReportId",
                        column: x => x.CleaningReportId,
                        principalTable: "CleaningReports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningReportItems_BuildingId",
                table: "CleaningReportItems",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningReportItems_CleaningReportId",
                table: "CleaningReportItems",
                column: "CleaningReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningReportItems");
        }
    }
}
