using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class ChangeProperties_MobileAppVersion_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url_Android",
                table: "MobileAppVersions");

            migrationBuilder.RenameColumn(
                name: "Url_iOS",
                table: "MobileAppVersions",
                newName: "Url");

            migrationBuilder.AddColumn<short>(
                name: "Platform",
                table: "MobileAppVersions",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "MobileAppVersions");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "MobileAppVersions",
                newName: "Url_iOS");

            migrationBuilder.AddColumn<string>(
                name: "Url_Android",
                table: "MobileAppVersions",
                nullable: true);
        }
    }
}
