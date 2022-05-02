using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class FreshdeskDefaultApiKeyToCompanySettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreshdeskDefaultApiKey",
                table: "CompanySettings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_FreshdeskDefaultApiKey",
                table: "CompanySettings",
                column: "FreshdeskDefaultApiKey",
                unique: true,
                filter: "[FreshdeskDefaultApiKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanySettings_FreshdeskDefaultApiKey",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "FreshdeskDefaultApiKey",
                table: "CompanySettings");
        }
    }
}
