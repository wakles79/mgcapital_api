using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveEFProposalServiceBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalServices_Buildings_BuildingId",
                table: "ProposalServices");

            migrationBuilder.DropIndex(
                name: "IX_ProposalServices_BuildingId",
                table: "ProposalServices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProposalServices_BuildingId",
                table: "ProposalServices",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalServices_Buildings_BuildingId",
                table: "ProposalServices",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
