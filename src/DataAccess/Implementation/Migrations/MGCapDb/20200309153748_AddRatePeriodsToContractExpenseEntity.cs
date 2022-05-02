using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddRatePeriodsToContractExpenseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyRate",
                table: "ContractExpenses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyRate",
                table: "ContractExpenses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyRate",
                table: "ContractExpenses",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyRate",
                table: "ContractExpenses");

            migrationBuilder.DropColumn(
                name: "MonthlyRate",
                table: "ContractExpenses");

            migrationBuilder.DropColumn(
                name: "YearlyRate",
                table: "ContractExpenses");
        }
    }
}
