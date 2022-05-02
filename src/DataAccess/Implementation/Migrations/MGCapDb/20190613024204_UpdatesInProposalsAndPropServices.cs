using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class UpdatesInProposalsAndPropServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Customers_CustomerId",
                table: "Proposals");

            migrationBuilder.AddColumn<string>(
                name: "BuildingName",
                table: "ProposalServices",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Proposals",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "BillToEmail",
                table: "Proposals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillToName",
                table: "Proposals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Proposals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Proposals",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Customers_CustomerId",
                table: "Proposals",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Customers_CustomerId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "BuildingName",
                table: "ProposalServices");

            migrationBuilder.DropColumn(
                name: "BillToEmail",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "BillToName",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Proposals");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Proposals",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Customers_CustomerId",
                table: "Proposals",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
