using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddExpenseTypesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    ExpenseCategory = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 128, nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    RateType = table.Column<int>(nullable: false),
                    Periodicity = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExpenseTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubcategories_ExpenseTypeId",
                table: "ExpenseSubcategories",
                column: "ExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTypes_CompanyId",
                table: "ExpenseTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseSubcategories_ExpenseTypes_ExpenseTypeId",
                table: "ExpenseSubcategories",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseSubcategories_ExpenseTypes_ExpenseTypeId",
                table: "ExpenseSubcategories");

            migrationBuilder.DropTable(
                name: "ExpenseTypes");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseSubcategories_ExpenseTypeId",
                table: "ExpenseSubcategories");
        }
    }
}
