using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddNewIsExpiredAnnotationWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsExpired",
                table: "WorkOrders",
                nullable: false,
                computedColumnSql: "CASE WHEN [DueDate] IS NULL THEN 0 WHEN (CASE WHEN [DueDate] = '0001-01-01 00:00:00.0000000' THEN '3000-01-01' ELSE CAST([DueDate] AS DATETIME) END) < GETUTCDATE() THEN 1 ELSE 0 END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "WorkOrders");
        }
    }
}
