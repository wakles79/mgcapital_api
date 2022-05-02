using MGCap.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddNewWorkOrderSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string query = "";
            query += $" INSERT INTO [WorkOrderSources] ([Name],[Code]) VALUES ('Ticket', {(int)WorkOrderSourceCode.Ticket})";
            query += $" INSERT INTO [WorkOrderSources] ([Name],[Code]) VALUES ('Calendar', {(int) WorkOrderSourceCode.Calendar})";
            migrationBuilder.Sql(query);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
