using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddForeignKeyToInspectionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InspectionId",
                table: "InspectionItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionItems_InspectionId",
                table: "InspectionItems",
                column: "InspectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionItems_Inspections_InspectionId",
                table: "InspectionItems",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionItems_Inspections_InspectionId",
                table: "InspectionItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionItems_InspectionId",
                table: "InspectionItems");

            migrationBuilder.DropColumn(
                name: "InspectionId",
                table: "InspectionItems");
        }
    }
}
