using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddWorkOrderServiceToTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "WorkOrderTasks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UnitFactor",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderServiceCategoryId",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderServiceId",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_WorkOrderServiceCategoryId",
                table: "WorkOrderTasks",
                column: "WorkOrderServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_WorkOrderServiceId",
                table: "WorkOrderTasks",
                column: "WorkOrderServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_WorkOrderServiceCategories_WorkOrderServiceCategoryId",
                table: "WorkOrderTasks",
                column: "WorkOrderServiceCategoryId",
                principalTable: "WorkOrderServiceCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_WorkOrderServices_WorkOrderServiceId",
                table: "WorkOrderTasks",
                column: "WorkOrderServiceId",
                principalTable: "WorkOrderServices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_WorkOrderServiceCategories_WorkOrderServiceCategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_WorkOrderServices_WorkOrderServiceId",
                table: "WorkOrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderTasks_WorkOrderServiceCategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderTasks_WorkOrderServiceId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "UnitFactor",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "WorkOrderServiceCategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "WorkOrderServiceId",
                table: "WorkOrderTasks");
        }
    }
}
