using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class RenameClientApprovedInScheduleSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClienteApproved",
                table: "WorkOrders",
                newName: "ClientApproved");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientApproved",
                table: "WorkOrders",
                newName: "ClienteApproved");
        }
    }
}
