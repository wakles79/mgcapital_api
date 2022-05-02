using MGCap.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class NewReadTaskBillingWorkOrderPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new permission
            migrationBuilder.Sql(string.Format(
                "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
                "'ReadWorkOrderTaskBillingInformationFromDetail'",
                (int)ApplicationModule.WorkOrder,
                (int)ActionType.Read));

            // Assign to master
            string query = $@"
                DECLARE @PermissionId INT 
                DECLARE @RoleId INT 
                SELECT TOP 1 @RoleId = [ID] FROM [Roles] WHERE [Level] = 10 AND [Type] = 0 ORDER BY ID ASC
                SELECT TOP 1 @PermissionId = [ID] FROM [Permissions] WHERE [Name] = 'ReadWorkOrderTaskBillingInformationFromDetail'

                SELECT * FROM PermissionRoles
                INSERT INTO [PermissionRoles] ([PermissionId],[RoleId]) VALUES (@PermissionId, @RoleId)";
            migrationBuilder.Sql(query);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
