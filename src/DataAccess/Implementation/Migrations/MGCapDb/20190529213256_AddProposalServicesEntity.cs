using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddProposalServicesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProposalServices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    ProposalId = table.Column<int>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    OfficeServiceTypeId = table.Column<int>(nullable: false),
                    Quantity = table.Column<string>(nullable: true),
                    RequesterName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    FinishDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalServices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProposalServices_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProposalServices_OfficeServiceTypes_OfficeServiceTypeId",
                        column: x => x.OfficeServiceTypeId,
                        principalTable: "OfficeServiceTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ProposalServices_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProposalServices_BuildingId",
                table: "ProposalServices",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalServices_OfficeServiceTypeId",
                table: "ProposalServices",
                column: "OfficeServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalServices_ProposalId",
                table: "ProposalServices",
                column: "ProposalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalServices");
        }
    }
}
