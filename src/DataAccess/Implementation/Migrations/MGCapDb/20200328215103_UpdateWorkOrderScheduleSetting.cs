using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class UpdateWorkOrderScheduleSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderScheduleSetting_WorkOrders_WorkOrderId",
                table: "WorkOrderScheduleSetting");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderScheduleSetting_WorkOrderId",
                table: "WorkOrderScheduleSetting");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "WorkOrderScheduleSetting");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrderScheduleSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderScheduleSetting_WorkOrderId",
                table: "WorkOrderScheduleSetting",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderScheduleSetting_WorkOrders_WorkOrderId",
                table: "WorkOrderScheduleSetting",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
