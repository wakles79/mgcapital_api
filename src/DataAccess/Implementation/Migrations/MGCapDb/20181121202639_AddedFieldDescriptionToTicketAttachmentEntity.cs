using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddedFieldDescriptionToTicketAttachmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TicketAttachments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CleaningReportItemAttachments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TicketAttachments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CleaningReportItemAttachments");
        }
    }
}
