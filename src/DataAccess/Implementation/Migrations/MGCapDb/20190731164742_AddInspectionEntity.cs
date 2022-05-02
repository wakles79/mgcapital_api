using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddInspectionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    SnoozeDate = table.Column<DateTime>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    Stars = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: true),
                    CloseDate = table.Column<DateTime>(nullable: true),
                    ClosingNotes = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Inspections_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Inspections_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_BuildingId",
                table: "Inspections",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_CompanyId",
                table: "Inspections",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspections");
        }
    }
}
