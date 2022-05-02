--DECLARE @userEmail AS NVARCHAR(128) = ''

SELECT 
	[dbo].[CustomerUsers].[Id] AS [CustomerUserId],
	[dbo].[CustomerUsers].[Email] AS [Email],
	[dbo].[CustomerUsers].[FirstName] AS [FirstName],
	[dbo].[CustomerUsers].[MiddleName] AS [MiddleName],
	[dbo].[CustomerUsers].[LastName] AS [LastName],
	[dbo].[CustomerUsers].[CustomerId] AS [CustomerId],
	[dbo].[Customers].[Name] AS [CustomerName],
	[dbo].[CustomerUsers].[CompanyId] AS [CompanyId],
	[dbo].[Companies].[Name] AS [CompanyName],
	[dbo].[Roles].[Level] AS [RoleLevel]
FROM
	[dbo].[CustomerUsers]
	INNER JOIN [dbo].[Customers] ON [dbo].[CustomerUsers].[CustomerId] = [dbo].[Customers].[Id]
	INNER JOIN [dbo].[Companies] ON [dbo].[CustomerUsers].[CompanyId] = [dbo].[Companies].[Id]
	LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[CustomerUsers].[RoleId] = [dbo].[Roles].[Id]
WHERE 
	[dbo].[CustomerUsers].[Email] = @userEmail
