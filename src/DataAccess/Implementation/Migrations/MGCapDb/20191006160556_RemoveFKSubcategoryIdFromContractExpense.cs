using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveFKSubcategoryIdFromContractExpense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractExpenses_ExpenseSubcategories_ExpenseSubcategoryId",
                table: "ContractExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ContractExpenses_ExpenseSubcategoryId",
                table: "ContractExpenses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContractExpenses_ExpenseSubcategoryId",
                table: "ContractExpenses",
                column: "ExpenseSubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractExpenses_ExpenseSubcategories_ExpenseSubcategoryId",
                table: "ContractExpenses",
                column: "ExpenseSubcategoryId",
                principalTable: "ExpenseSubcategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
