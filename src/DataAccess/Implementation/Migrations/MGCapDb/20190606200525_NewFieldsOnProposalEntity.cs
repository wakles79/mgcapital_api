using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class NewFieldsOnProposalEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateToDelivery",
                table: "ProposalServices",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "BillTo",
                table: "Proposals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedDate",
                table: "Proposals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillTo",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "StatusChangedDate",
                table: "Proposals");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateToDelivery",
                table: "ProposalServices",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
