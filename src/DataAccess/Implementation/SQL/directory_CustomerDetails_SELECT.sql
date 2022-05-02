--DECLARE @customerId AS INT = 158

SELECT 
	[dbo].[Customers].[ID],
	[dbo].[Customers].[Name],
	[dbo].[Customers].[Notes],
	-- Buildings
	ISNULL([dbo].[Buildings].[ID], -1) AS [Id],
	[dbo].[Buildings].[Name],
	-- Contacts
	ISNULL([dbo].[Contacts].[ID], -1) AS [Id],
	CONCAT([Contacts].[FirstName], ' ', [Contacts].[MiddleName], ' ', [Contacts].[LastName]) AS [Data], 
	[dbo].[CustomerContacts].[Type] AS [Label]

FROM
	[dbo].[Customers]
	LEFT OUTER JOIN [dbo].[Contracts] ON [dbo].[Customers].[Id] = [dbo].[Contracts].[CustomerId]
	LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[Buildings].[Id] = [dbo].[Contracts].[BuildingId]
	LEFT OUTER JOIN [dbo].[CustomerContacts] ON [dbo].[Customers].[ID] = [dbo].[CustomerContacts].[CustomerId]
	LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[CustomerContacts].[ContactId] = [dbo].[Contacts].[ID]
WHERE
	[dbo].[Customers].[ID] = @customerId

