using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class ChangeIsExpiredWOEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN (CASE WHEN [DueDate] = '0001-01-01 00:00:00.0000000' THEN '3000-01-01' ELSE [DueDate] END) < GETDATE() THEN 1 ELSE 0 END",
                oldClrType: typeof(int),
                oldComputedColumnSql: "CASE WHEN ISNULL([DueDate], '2000-01-01') < GETDATE() THEN 1 ELSE 0 END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN ISNULL([DueDate], '2000-01-01') < GETDATE() THEN 1 ELSE 0 END",
                oldClrType: typeof(int),
                oldComputedColumnSql: "CASE WHEN (CASE WHEN [DueDate] = '0001-01-01 00:00:00.0000000' THEN '3000-01-01' ELSE [DueDate] END) < GETDATE() THEN 1 ELSE 0 END");
        }
    }
}
