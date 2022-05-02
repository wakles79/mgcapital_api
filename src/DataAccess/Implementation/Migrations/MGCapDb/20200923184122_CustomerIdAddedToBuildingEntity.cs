using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class CustomerIdAddedToBuildingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Buildings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CustomerId",
                table: "Buildings",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Customers_CustomerId",
                table: "Buildings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Customers_CustomerId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_CustomerId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Buildings");
        }
    }
}
