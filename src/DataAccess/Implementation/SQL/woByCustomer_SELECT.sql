-- dynamic parameters
DECLARE @companyId AS INT = 1
DECLARE @userEmail AS NVARCHAR(100) = 'mgclient.one@testing.com'

-- embedded query

DECLARE @customerId AS INT;

SELECT @customerId = ( 
						SELECT [dbo].[CustomerUsers].[CustomerId] 
						FROM [dbo].[CustomerUsers] 
						WHERE [dbo].[CustomerUsers].[Email] = @userEmail AND [dbo].[CustomerUsers].[CompanyId] = @companyId 
					 )

SELECT 
	[dbo].[WorkOrders].[Id] AS [Id],
	[dbo].[WorkOrders].[Number] AS [Number],
	[dbo].[Buildings].[Name] AS [BuildingName],
	[dbo].[WorkOrders].[Location] AS [Location],
	[dbo].[WorkOrders].[Description] AS [Description],
	[dbo].[WorkOrders].[StatusId] AS [StatusId]
	, [dbo].[WorkOrders].[CreatedDate]

FROM
	[dbo].[WorkOrders]
	INNER JOIN [dbo].[Buildings] ON [dbo].[Buildings].[Id] = [dbo].[WorkOrders].[BuildingId]
	INNER JOIN [dbo].[Contracts] ON [dbo].[Contracts].[BuildingId] = [dbo].[WorkOrders].[BuildingId]
									AND [dbo].[Contracts].[CustomerId] = @customerId
									AND [dbo].[Contracts].[Status] = 1

WHERE 
	[dbo].[WorkOrders].[CompanyId] = @companyId
	--AND [dbo].[WorkOrders].[StatusId] IN (2,3)
	--AND CAST([dbo].[WorkOrders].[CreatedDate] AS DATE) >= '2018-11-02 00:00:00'
	
