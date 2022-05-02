using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddScheduleSettingToWOEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ClienteApproved",
                table: "WorkOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleCategoryId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ScheduleDateConfirmed",
                table: "WorkOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleSubCategoryId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduleCategoryId",
                table: "WorkOrders",
                column: "ScheduleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduleSubCategoryId",
                table: "WorkOrders",
                column: "ScheduleSubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ScheduleSettingCategories_ScheduleCategoryId",
                table: "WorkOrders",
                column: "ScheduleCategoryId",
                principalTable: "ScheduleSettingCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ScheduleSettingSubCategories_ScheduleSubCategoryId",
                table: "WorkOrders",
                column: "ScheduleSubCategoryId",
                principalTable: "ScheduleSettingSubCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ScheduleSettingCategories_ScheduleCategoryId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ScheduleSettingSubCategories_ScheduleSubCategoryId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ScheduleCategoryId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ScheduleSubCategoryId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClienteApproved",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduleCategoryId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduleDateConfirmed",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduleSubCategoryId",
                table: "WorkOrders");
        }
    }
}
