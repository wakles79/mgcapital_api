using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedRolesAndPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN [DueDate] <> '0001-01-01 00:00:00.0000000' AND [DueDate] < GETDATE() THEN 1 ELSE 0 END",
                oldClrType: typeof(bool),
                oldComputedColumnSql: "CASE WHEN [DueDate] <> '0001-01-01 00:00:00.0000000' AND [DueDate] < GETDATE() THEN 1 ELSE 0 END");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.AlterColumn<bool>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN [DueDate] <> '0001-01-01 00:00:00.0000000' AND [DueDate] < GETDATE() THEN 1 ELSE 0 END",
                oldClrType: typeof(int),
                oldComputedColumnSql: "CASE WHEN [DueDate] <> '0001-01-01 00:00:00.0000000' AND [DueDate] < GETDATE() THEN 1 ELSE 0 END");
        }
    }
}
