using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RenamedWOSendCustomerNotificationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendCustomerNotifications",
                table: "WorkOrders",
                newName: "SendRequesterNotifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendRequesterNotifications",
                table: "WorkOrders",
                newName: "SendCustomerNotifications");
        }
    }
}
