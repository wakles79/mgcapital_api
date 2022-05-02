using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveUnusedPropertiesBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DailyRate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyRate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearlyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearlyRate",
                table: "Contracts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DailyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyRate",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
