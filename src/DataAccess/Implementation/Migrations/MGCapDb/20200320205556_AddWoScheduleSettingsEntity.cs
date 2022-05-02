using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddWoScheduleSettingsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrderScheduleSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    WorkOrderId = table.Column<int>(nullable: false),
                    Frequency = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Ordinal = table.Column<int>(nullable: true),
                    StartValue = table.Column<int>(nullable: true),
                    EndValue = table.Column<int>(nullable: true),
                    Days = table.Column<string>(nullable: true),
                    ScheduleDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderScheduleSetting", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkOrderScheduleSetting_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderScheduleSetting_WorkOrderId",
                table: "WorkOrderScheduleSetting",
                column: "WorkOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderScheduleSetting");
        }
    }
}
