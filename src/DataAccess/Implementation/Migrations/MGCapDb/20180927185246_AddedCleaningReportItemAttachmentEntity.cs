using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedCleaningReportItemAttachmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleaningReportItemAttachments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlobName = table.Column<string>(nullable: true),
                    CleaningReportItemId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FullUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningReportItemAttachments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CleaningReportItemAttachments_CleaningReportItems_CleaningReportItemId",
                        column: x => x.CleaningReportItemId,
                        principalTable: "CleaningReportItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningReportItemAttachments_CleaningReportItemId",
                table: "CleaningReportItemAttachments",
                column: "CleaningReportItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningReportItemAttachments");
        }
    }
}
