using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class Added_PushNotifications_Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PushNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompletedAt = table.Column<int>(nullable: false),
                    Content = table.Column<string>(maxLength: 250, nullable: true),
                    Converted = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    DataType = table.Column<int>(nullable: false),
                    Heading = table.Column<string>(maxLength: 80, nullable: true),
                    OneSignalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotifications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationConverts",
                columns: table => new
                {
                    PushNotificationId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationConverts", x => new { x.PushNotificationId, x.EmployeeId });
                    table.UniqueConstraint("AK_PushNotificationConverts_EmployeeId_PushNotificationId", x => new { x.EmployeeId, x.PushNotificationId });
                    table.ForeignKey(
                        name: "FK_PushNotificationConverts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushNotificationConverts_PushNotifications_PushNotificationId",
                        column: x => x.PushNotificationId,
                        principalTable: "PushNotifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationFilters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Field = table.Column<string>(maxLength: 256, nullable: true),
                    Key = table.Column<string>(maxLength: 256, nullable: true),
                    PushNotificationId = table.Column<int>(nullable: false),
                    Relation = table.Column<string>(maxLength: 3, nullable: true),
                    Value = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationFilters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PushNotificationFilters_PushNotifications_PushNotificationId",
                        column: x => x.PushNotificationId,
                        principalTable: "PushNotifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationFilters_PushNotificationId",
                table: "PushNotificationFilters",
                column: "PushNotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushNotificationConverts");

            migrationBuilder.DropTable(
                name: "PushNotificationFilters");

            migrationBuilder.DropTable(
                name: "PushNotifications");
        }
    }
}
