using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddTicketEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    Source = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    DestinationType = table.Column<int>(nullable: false),
                    DestinationEntityId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FullAddress = table.Column<string>(maxLength: 250, nullable: true),
                    BuildingId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    RequesterFullName = table.Column<string>(maxLength: 200, nullable: true),
                    RequesterEmail = table.Column<string>(maxLength: 128, nullable: true),
                    RequesterPhone = table.Column<string>(maxLength: 13, nullable: true),
                    SnoozeDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tickets_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BuildingId",
                table: "Tickets",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CompanyId",
                table: "Tickets",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
