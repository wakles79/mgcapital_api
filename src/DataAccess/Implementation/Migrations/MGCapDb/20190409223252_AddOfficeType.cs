using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
	public partial class AddOfficeType : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "OfficeType",
				columns: table => new
				{
					ID = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					CompanyId = table.Column<int>(nullable: false),
					CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
					CreatedDate = table.Column<DateTime>(nullable: false),
					Guid = table.Column<Guid>(nullable: false),
					Name = table.Column<string>(maxLength: 64, nullable: false),
					Rate = table.Column<double>(nullable: false),
					UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
					UpdatedDate = table.Column<DateTime>(nullable: false)
				}, constraints: table => 
				{
					table.PrimaryKey("PK_OfficeType",x=>x.ID);
					table.ForeignKey(
						name: "FK_OfficeType_Companies_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Companies",
						principalColumn: "ID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_OfficeType_CompanyId",
				table: "OfficeType",
				column: "CompanyId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "OfficeType");
		}
	}
}
