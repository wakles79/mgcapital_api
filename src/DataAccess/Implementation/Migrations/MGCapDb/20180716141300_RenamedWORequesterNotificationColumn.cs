using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RenamedWORequesterNotificationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendRequesterNotifications",
                table: "WorkOrders",
                newName: "SendPropertyManagersNotifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendPropertyManagersNotifications",
                table: "WorkOrders",
                newName: "SendRequesterNotifications");
        }
    }
}
