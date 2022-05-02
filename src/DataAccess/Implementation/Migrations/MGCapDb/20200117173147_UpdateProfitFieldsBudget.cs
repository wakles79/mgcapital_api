using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class UpdateProfitFieldsBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DailyProfitRatio",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyProfitRatio",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyProfit",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyProfitRatio",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DailyProfitRatio",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MonthlyProfitRatio",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearlyProfit",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearlyProfitRatio",
                table: "Contracts");
        }
    }
}
