using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class UpdatesInContractsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DaysPerMonth",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                 name: "DailyRate",
                 table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DaysPerMonth",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyRate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearlyRate",
                table: "Contracts");
        }
    }
}
