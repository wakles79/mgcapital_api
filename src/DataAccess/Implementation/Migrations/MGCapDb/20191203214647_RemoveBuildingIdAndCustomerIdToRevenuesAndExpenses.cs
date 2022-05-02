using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveBuildingIdAndCustomerIdToRevenuesAndExpenses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Buildings_BuildingId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Customers_CustomerId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Revenues_Buildings_BuildingId",
                table: "Revenues");

            migrationBuilder.DropForeignKey(
                name: "FK_Revenues_Customers_CustomerId",
                table: "Revenues");

            migrationBuilder.DropIndex(
                name: "IX_Revenues_BuildingId",
                table: "Revenues");

            migrationBuilder.DropIndex(
                name: "IX_Revenues_CustomerId",
                table: "Revenues");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BuildingId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CustomerId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Revenues");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Revenues");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Expenses");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionNumber",
                table: "Revenues",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionNumber",
                table: "Expenses",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionNumber",
                table: "Revenues",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Revenues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Revenues",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionNumber",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Revenues_BuildingId",
                table: "Revenues",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Revenues_CustomerId",
                table: "Revenues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BuildingId",
                table: "Expenses",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CustomerId",
                table: "Expenses",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Buildings_BuildingId",
                table: "Expenses",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Customers_CustomerId",
                table: "Expenses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Revenues_Buildings_BuildingId",
                table: "Revenues",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Revenues_Customers_CustomerId",
                table: "Revenues",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
