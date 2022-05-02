using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedComputedColumnToBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsAvailableForContract",
                table: "Buildings",
                nullable: false,
                computedColumnSql: "CASE WHEN [OperationsManagerId] IS NOT NULL AND [IsActive] = 1 THEN 1 ELSE 0 END",
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsAvailableForContract",
                table: "Buildings",
                nullable: false);
        }
    }
}
