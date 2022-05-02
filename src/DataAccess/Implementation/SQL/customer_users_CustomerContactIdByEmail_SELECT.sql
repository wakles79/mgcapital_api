DECLARE @userEmail AS NVARCHAR(128) = 'mgclient.ten@testing.com'

SELECT
	ISNULL([dbo].[ContactEmails].[Id], -1) AS [ContactEmailId],
	ISNULL([dbo].[CustomerContacts].[ContactId], -1) AS [ContactId],
	ISNULL([dbo].[Customers].[Id], -1) AS [CustomerId]
FROM
	[dbo].[ContactEmails]
	INNER JOIN [dbo].[CustomerContacts] ON [dbo].[ContactEmails].[ContactId] = [dbo].[CustomerContacts].[ContactId]
	INNER JOIN [dbo].[Customers] ON [dbo].[CustomerContacts].[CustomerId] = [dbo].[Customers].[Id]
WHERE
	[dbo].[ContactEmails].[Email] = @userEmail 
	AND [dbo].[Customers].[CompanyId] = 1
	