using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RenameOfficeTypeTableSecondTry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficeType_Companies_CompanyId",
                table: "OfficeType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeType",
                table: "OfficeType");

            migrationBuilder.RenameTable(
                name: "OfficeType",
                newName: "OfficeTypes");

            migrationBuilder.RenameIndex(
                name: "IX_OfficeType_CompanyId",
                table: "OfficeTypes",
                newName: "IX_OfficeTypes_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeTypes",
                table: "OfficeTypes",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeTypes_Companies_CompanyId",
                table: "OfficeTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficeTypes_Companies_CompanyId",
                table: "OfficeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeTypes",
                table: "OfficeTypes");

            migrationBuilder.RenameTable(
                name: "OfficeTypes",
                newName: "OfficeType");

            migrationBuilder.RenameIndex(
                name: "IX_OfficeTypes_CompanyId",
                table: "OfficeType",
                newName: "IX_OfficeType_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeType",
                table: "OfficeType",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeType_Companies_CompanyId",
                table: "OfficeType",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
