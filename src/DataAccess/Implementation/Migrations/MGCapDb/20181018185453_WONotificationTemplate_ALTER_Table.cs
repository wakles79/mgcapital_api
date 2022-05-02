using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class WONotificationTemplate_ALTER_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SMSTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "SubjectTemplate");

            migrationBuilder.RenameColumn(
                name: "PlainEmailTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "RichtextBodyTemplate");

            migrationBuilder.RenameColumn(
                name: "HTMLEmailTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "PlainTextTemplate");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "WorkOrderNotificationTemplates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "WorkOrderNotificationTemplates");

            migrationBuilder.RenameColumn(
                name: "SubjectTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "SMSTemplate");

            migrationBuilder.RenameColumn(
                name: "RichtextBodyTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "PlainEmailTemplate");

            migrationBuilder.RenameColumn(
                name: "PlainTextTemplate",
                table: "WorkOrderNotificationTemplates",
                newName: "HTMLEmailTemplate");
        }
    }
}
