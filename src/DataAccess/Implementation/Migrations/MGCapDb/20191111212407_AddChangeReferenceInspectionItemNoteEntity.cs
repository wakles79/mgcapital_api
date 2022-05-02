using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddChangeReferenceInspectionItemNoteEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionItemNotes_Inspections_InspectionItemId",
                table: "InspectionItemNotes");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionItemNotes_InspectionItems_InspectionItemId",
                table: "InspectionItemNotes",
                column: "InspectionItemId",
                principalTable: "InspectionItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionItemNotes_InspectionItems_InspectionItemId",
                table: "InspectionItemNotes");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionItemNotes_Inspections_InspectionItemId",
                table: "InspectionItemNotes",
                column: "InspectionItemId",
                principalTable: "Inspections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
