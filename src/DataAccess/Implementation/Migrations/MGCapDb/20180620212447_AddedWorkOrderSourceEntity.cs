using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedWorkOrderSourceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "WorkOrders");

            migrationBuilder.AddColumn<int>(
                name: "WOSourceId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkOrderSource",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderSource", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WOSourceId",
                table: "WorkOrders",
                column: "WOSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_WorkOrderSource_WOSourceId",
                table: "WorkOrders",
                column: "WOSourceId",
                principalTable: "WorkOrderSource",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_WorkOrderSource_WOSourceId",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "WorkOrderSource");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_WOSourceId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "WOSourceId",
                table: "WorkOrders");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "WorkOrders",
                nullable: false,
                defaultValue: 0);
        }
    }
}
