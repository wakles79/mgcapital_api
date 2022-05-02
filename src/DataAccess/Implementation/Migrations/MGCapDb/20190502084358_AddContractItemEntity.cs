using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddContractItemEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractItems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 128, nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    OfficeServiceTypeId = table.Column<int>(nullable: false),
                    OfficeServiceTypeName = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    RateType = table.Column<int>(nullable: false),
                    RatePeriodicity = table.Column<string>(maxLength: 8, nullable: true),
                    Hours = table.Column<double>(nullable: true),
                    Amount = table.Column<double>(nullable: true),
                    Rooms = table.Column<double>(nullable: true),
                    SquareFeet = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContractItems_OfficeServiceTypes_OfficeServiceTypeId",
                        column: x => x.OfficeServiceTypeId,
                        principalTable: "OfficeServiceTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractItems_OfficeServiceTypeId",
                table: "ContractItems",
                column: "OfficeServiceTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractItems");
        }
    }
}
