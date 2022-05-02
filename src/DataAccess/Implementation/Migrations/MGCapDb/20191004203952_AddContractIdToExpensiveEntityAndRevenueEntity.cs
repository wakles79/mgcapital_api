using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddContractIdToExpensiveEntityAndRevenueEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Revenues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Revenues_ContractId",
                table: "Revenues",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ContractId",
                table: "Expenses",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Contracts_ContractId",
                table: "Expenses",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Revenues_Contracts_ContractId",
                table: "Revenues",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Contracts_ContractId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Revenues_Contracts_ContractId",
                table: "Revenues");

            migrationBuilder.DropIndex(
                name: "IX_Revenues_ContractId",
                table: "Revenues");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ContractId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Revenues");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Expenses");
        }
    }
}
