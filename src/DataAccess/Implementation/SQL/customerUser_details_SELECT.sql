DECLARE @userId AS INT = 1
DECLARE @companyId AS INT = 1

SELECT
	[dbo].[CustomerUsers].[Id] AS [Id],
	[dbo].[CustomerUsers].[Email] AS [Email],
	[dbo].[Contacts].[FirstName] AS [FirstName],
	[dbo].[Contacts].[MiddleName] AS [MiddleName],
	[dbo].[Contacts].[LastName] AS [LastName]
FROM
	[dbo].[CustomerUsers]
	LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[CustomerUsers].[ContactId] = [dbo].[Contacts].[Id]
WHERE
	[dbo].[CustomerUsers].[Id] = @userId AND [dbo].[CustomerUsers].[CompanyId] = @companyId
