DECLARE @companyId AS INT = 1

SELECT
	[dbo].[Employees].[Id],
	[dbo].[Employees].[Email] AS [UserEmail],
	(
		SELECT 
			COUNT([dbo].[WorkOrderEmployees].[WorkOrderId]) 
		FROM 
			[dbo].[WorkOrders]
			INNER JOIN [dbo].[WorkOrderEmployees] 
				ON [dbo].[WorkOrderEmployees].[WorkOrderId] = [dbo].[WorkOrders].[Id] 
					AND [dbo].[WorkOrderEmployees].[EmployeeId] = [dbo].[Employees].[Id]
		WHERE 
			[dbo].[WorkOrders].[StatusId] IN (1, 2) AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) = CAST(GETDATE() AS DATE)
	) AS [DueToday],
	(
		SELECT 
			COUNT([dbo].[WorkOrderEmployees].[WorkOrderId]) 
		FROM 
			[dbo].[WorkOrders]
			INNER JOIN [dbo].[WorkOrderEmployees] 
				ON [dbo].[WorkOrderEmployees].[WorkOrderId] = [dbo].[WorkOrders].[Id] 
					AND [dbo].[WorkOrderEmployees].[EmployeeId] = [dbo].[Employees].[Id]
		WHERE 
			[dbo].[WorkOrders].[StatusId] IN (1, 2) AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) < CAST(GETDATE() AS DATE)
	) AS [PastDue]
FROM
	[dbo].[Employees]
	INNER JOIN [dbo].[Roles] ON [dbo].[Roles].[Id] = [dbo].[Employees].[RoleId]
WHERE
	[dbo].[Employees].[CompanyId] = @companyId AND
	[dbo].[Roles].[Level] = 30
