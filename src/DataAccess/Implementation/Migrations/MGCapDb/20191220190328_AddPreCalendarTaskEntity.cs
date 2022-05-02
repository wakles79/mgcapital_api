using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddPreCalendarTaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreCalendarTasks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    PreCalendarId = table.Column<int>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ServiceId = table.Column<int>(nullable: true),
                    UnitPrice = table.Column<double>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    DiscountPercentage = table.Column<double>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    LastCheckedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreCalendarTasks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PreCalendarTasks_PreCalendar_PreCalendarId",
                        column: x => x.PreCalendarId,
                        principalTable: "PreCalendar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreCalendarTasks_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreCalendarTasks_PreCalendarId",
                table: "PreCalendarTasks",
                column: "PreCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_PreCalendarTasks_ServiceId",
                table: "PreCalendarTasks",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreCalendarTasks");
        }
    }
}
