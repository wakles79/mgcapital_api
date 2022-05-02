using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedFullAddressComputedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "Addresses",
                nullable: true,
                computedColumnSql: "CONCAT(AddressLine1 + ' ', AddressLine2 + ' ', City + ' ', State + ' ', ZipCode + ' ', CountryCode)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "Addresses");
        }
    }
}
