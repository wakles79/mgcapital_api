using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RemoveIsExpiredAndNullDueDateWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "WorkOrders",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "WorkOrders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN (CASE WHEN [DueDate] = '0001-01-01 00:00:00.0000000' THEN '3000-01-01' ELSE CAST([DueDate] AS DATETIME) END) < GETUTCDATE() THEN 1 ELSE 0 END");
        }
    }
}
