using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class UpdatesInExpenseTypeAndExpenseSubcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Periodicity",
                table: "ExpenseTypes");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ExpenseTypes");

            migrationBuilder.DropColumn(
                name: "RateType",
                table: "ExpenseTypes");           

            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "ExpenseSubcategories",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RateType",
                table: "ExpenseSubcategories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
               name: "Periodicity",
               table: "ExpenseSubcategories",
               nullable: false,
               defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ExpenseSubcategories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Periodicity",
                table: "ExpenseSubcategories");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ExpenseSubcategories");

            migrationBuilder.DropColumn(
                name: "RateType",
                table: "ExpenseSubcategories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExpenseSubcategories");

            migrationBuilder.AddColumn<string>(
                name: "Periodicity",
                table: "ExpenseTypes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "ExpenseTypes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RateType",
                table: "ExpenseTypes",
                nullable: false,
                defaultValue: 0);
        }
    }
}
