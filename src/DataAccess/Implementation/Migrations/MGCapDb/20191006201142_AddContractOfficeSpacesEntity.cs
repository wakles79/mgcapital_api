using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddContractOfficeSpacesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractOfficeSpaces",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    OfficeTypeId = table.Column<int>(nullable: false),
                    SquareFeet = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOfficeSpaces", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContractOfficeSpaces_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOfficeSpaces_OfficeServiceTypes_OfficeTypeId",
                        column: x => x.OfficeTypeId,
                        principalTable: "OfficeServiceTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractOfficeSpaces_ContractId",
                table: "ContractOfficeSpaces",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOfficeSpaces_OfficeTypeId",
                table: "ContractOfficeSpaces",
                column: "OfficeTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractOfficeSpaces");
        }
    }
}
