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
	[dbo].[Buildings].[Id] AS [Id],
	[dbo].[Buildings].[Name] AS [Name]
FROM
	[dbo].[Buildings]
	INNER JOIN [dbo].[Contracts] ON [dbo].[Buildings].[Id] = [dbo].[Contracts].[BuildingId]
									AND [dbo].[Contracts].[CustomerId] = @customerId
									AND [dbo].[Contracts].[Status] = 1
WHERE
	[dbo].[Buildings].[CompanyId] = @companyId 
ORDER BY
	[dbo].[Buildings].[Name]
