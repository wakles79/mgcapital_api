using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddGmailEnabledColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HistoryId",
                table: "Tickets",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<bool>(
                name: "GmailEnabled",
                table: "CompanySettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GmailEnabled",
                table: "CompanySettings");

            migrationBuilder.AlterColumn<decimal>(
                name: "HistoryId",
                table: "Tickets",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
