using MGCap.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddWorkOrderServicesPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(string.Format(
                "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
                "'ReadWorkOrderServices'",
                (int)ApplicationModule.WorkOrderServicesCatalog,
                (int)ActionType.Read));

            //migrationBuilder.Sql(string.Format(
            //    "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
            //    "'ReadWorkOrderCategories'",
            //    (int)ApplicationModule.WorkOrderServicesCatalog,
            //    (int)ActionType.Read));
            //migrationBuilder.Sql(string.Format(
            //    "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
            //    "'AddWorkOrderCategories'",
            //    (int)ApplicationModule.WorkOrderServicesCatalog,
            //    (int)ActionType.Write));
            //migrationBuilder.Sql(string.Format(
            //    "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
            //    "'UpdateWorkOrderCategories'",
            //    (int)ApplicationModule.WorkOrderServicesCatalog,
            //    (int)ActionType.Write));
            migrationBuilder.Sql(string.Format(
                "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
                "'AddWorkOrderServices'",
                (int)ApplicationModule.WorkOrderServicesCatalog,
                (int)ActionType.Write));
            migrationBuilder.Sql(string.Format(
                "INSERT INTO [Permissions] ([Name], [Module], [Type]) VALUES ({0},{1},{2})",
                "'UpdateWorkOrderServices'",
                (int)ApplicationModule.WorkOrderServicesCatalog,
                (int)ActionType.Write));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(string.Format("DELETE FROM [Permissions] WHERE [Module] = {0}", (int)ApplicationModule.WorkOrderServicesCatalog));
        }
    }
}
