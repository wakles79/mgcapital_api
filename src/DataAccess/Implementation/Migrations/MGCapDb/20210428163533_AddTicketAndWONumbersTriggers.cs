using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddTicketAndWONumbersTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                --CREATE OR Alter Functions
                CREATE OR Alter FUNCTION [dbo].[GetTicketNextNumber](@companyId int) RETURNS int AS
                BEGIN    
                    DECLARE @NextNumber As int;
	                select @NextNumber= isnull(Max(Number),0) + 1 FROM [dbo].[Tickets] WITH (UPDLOCK, HOLDLOCK) where CompanyId = @companyId
                    RETURN @NextNumber;
                END
                GO
                CREATE OR Alter FUNCTION [dbo].[GetWorkOrderNextNumber](@companyId int) RETURNS int AS
                BEGIN    
                    DECLARE @NextNumber As int;
	                select @NextNumber= isnull(Max(Number),0) + 1 FROM [dbo].[WorkOrders] WITH (UPDLOCK, HOLDLOCK) where CompanyId = @companyId
                    RETURN @NextNumber;
                END
                GO
                CREATE OR Alter FUNCTION [dbo].[GetCleaningReportNextNumber](@companyId int) RETURNS int AS
                BEGIN    
                    DECLARE @NextNumber As int;
	                select @NextNumber= isnull(Max(Number),0) + 1 FROM [dbo].[CleaningReports] WITH (UPDLOCK, HOLDLOCK) where CompanyId = @companyId
                    RETURN @NextNumber;
                END
                GO
                CREATE OR Alter TRIGGER [dbo].[TicketNextNumberTrigger]
                ON [dbo].[Tickets]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
	                Update dbo.Tickets  
	                set Number = [dbo].[GetTicketNextNumber](i.CompanyId)
	                from inserted i
	                where Tickets.ID = i.ID and isnull(i.Number,0) = 0
                END
                GO
                CREATE OR Alter TRIGGER [dbo].[WorkOrderNumberTrigger]
                ON [dbo].[WorkOrders]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
	                Update dbo.WorkOrders  
	                set Number = [dbo].[GetWorkOrderNextNumber](i.CompanyId)
	                from inserted i
	                where WorkOrders.ID = i.ID and isnull(i.Number,0) = 0
                END
                GO
                CREATE OR Alter TRIGGER [dbo].[CleaningReportNextNumberTrigger]
                ON [dbo].[CleaningReports]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
	                Update dbo.CleaningReports  
	                set Number = [dbo].[GetCleaningReportNextNumber](i.CompanyId)
	                from inserted i
	                where CleaningReports.ID = i.ID and isnull(i.Number,0) = 0
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                --Create Functions
                IF EXISTS (SELECT *
                           FROM   sys.objects
                           WHERE  object_id = OBJECT_ID(N'[dbo].[GetTicketNextNumber]')
                                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
                  DROP FUNCTION [dbo].[GetTicketNextNumber]
                GO
                IF EXISTS (SELECT *
                           FROM   sys.objects
                           WHERE  object_id = OBJECT_ID(N'[dbo].[GetWorkOrderNextNumber]')
                                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
                  DROP FUNCTION [dbo].[GetWorkOrderNextNumber]
                GO
                IF EXISTS (SELECT *
                           FROM   sys.objects
                           WHERE  object_id = OBJECT_ID(N'[dbo].[GetCleaningReportNextNumber]')
                                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
                  DROP FUNCTION [dbo].[GetCleaningReportNextNumber]
                GO
                --Create TRIGGER
                IF exists (SELECT * FROM sys.objects WHERE [name] = N'TicketNextNumberTrigger' AND [type] = 'TR')
                BEGIN
                      DROP TRIGGER [dbo].[TicketNextNumberTrigger];
                END;
                GO
                IF exists (SELECT * FROM sys.objects WHERE [name] = N'WorkOrderNumberTrigger' AND [type] = 'TR')
                BEGIN
                      DROP TRIGGER [dbo].[WorkOrderNumberTrigger];
                END;
                GO
                IF exists (SELECT * FROM sys.objects WHERE [name] = N'CleaningReportNextNumberTrigger' AND [type] = 'TR')
                BEGIN
                      DROP TRIGGER [dbo].[CleaningReportNextNumberTrigger];
                END;
                GO
            ");
        }
    }
}
