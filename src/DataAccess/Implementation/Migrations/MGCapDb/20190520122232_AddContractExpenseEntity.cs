using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddContractExpenseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractExpenses",
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
                    ExpenseCategory = table.Column<int>(nullable: false),
                    ExpenseTypeName = table.Column<string>(nullable: true),
                    ExpenseSubcategoryId = table.Column<int>(nullable: false),
                    ExpenseSubcategoryName = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    RateType = table.Column<int>(nullable: false),
                    RatePeriodicity = table.Column<string>(maxLength: 8, nullable: true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractExpenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContractExpenses_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractExpenses_ExpenseSubcategories_ExpenseSubcategoryId",
                        column: x => x.ExpenseSubcategoryId,
                        principalTable: "ExpenseSubcategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractExpenses_ContractId",
                table: "ContractExpenses",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractExpenses_ExpenseSubcategoryId",
                table: "ContractExpenses",
                column: "ExpenseSubcategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractExpenses");
        }
    }
}
