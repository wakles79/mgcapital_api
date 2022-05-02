using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedNewFieldsToWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "WorkOrders",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterEmail",
                table: "WorkOrders",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterExt",
                table: "WorkOrders",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterFirstName",
                table: "WorkOrders",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterLastName",
                table: "WorkOrders",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterPhone",
                table: "WorkOrders",
                maxLength: 13,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterEmail",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterExt",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterFirstName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterLastName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequesterPhone",
                table: "WorkOrders");
        }
    }
}
