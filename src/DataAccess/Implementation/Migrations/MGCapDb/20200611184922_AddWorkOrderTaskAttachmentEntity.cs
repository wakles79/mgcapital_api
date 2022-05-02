using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddWorkOrderTaskAttachmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrderTaskAttachments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    BlobName = table.Column<string>(nullable: true),
                    FullUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    ImageTakenDate = table.Column<DateTime>(nullable: false),
                    WorkOrderTaskId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderTaskAttachments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkOrderTaskAttachments_WorkOrderTasks_WorkOrderTaskId",
                        column: x => x.WorkOrderTaskId,
                        principalTable: "WorkOrderTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTaskAttachments_WorkOrderTaskId",
                table: "WorkOrderTaskAttachments",
                column: "WorkOrderTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderTaskAttachments");
        }
    }
}
