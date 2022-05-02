using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddFreshdeskSettingToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreshdeskDefaultAgentId",
                table: "CompanySettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreshdeskEmail",
                table: "CompanySettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreshdeskDefaultAgentId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "FreshdeskEmail",
                table: "CompanySettings");
        }
    }
}
