using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddCategoryToWorkOrderTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubcategoryId",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_CategoryId",
                table: "WorkOrderTasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_SubcategoryId",
                table: "WorkOrderTasks",
                column: "SubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_ScheduleSettingCategories_CategoryId",
                table: "WorkOrderTasks",
                column: "CategoryId",
                principalTable: "ScheduleSettingCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_ScheduleSettingSubCategories_SubcategoryId",
                table: "WorkOrderTasks",
                column: "SubcategoryId",
                principalTable: "ScheduleSettingSubCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_ScheduleSettingCategories_CategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_ScheduleSettingSubCategories_SubcategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderTasks_CategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderTasks_SubcategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "SubcategoryId",
                table: "WorkOrderTasks");
        }
    }
}
