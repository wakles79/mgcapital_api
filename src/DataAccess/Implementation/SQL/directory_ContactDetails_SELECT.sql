--DECLARE @contactId AS INT = 201

SELECT 
	[dbo].[Contacts].[ID],
	CONCAT([Contacts].[FirstName], ' ', [Contacts].[MiddleName], ' ', [Contacts].[LastName]) AS [Name], 
	[dbo].[Contacts].[DOB] AS [Birthday], 
	[dbo].[Contacts].[Notes],
	-- Phones
	ISNULL([dbo].[ContactPhones].[ID], -1) AS [Id],
	[dbo].[ContactPhones].[Type] AS [Label],
	[dbo].[ContactPhones].[Phone] AS [Data],
	-- Emails
	ISNULL([dbo].[ContactEmails].[ID], -1) AS [Id],
	[dbo].[ContactEmails].[Type] AS [Label],
	[dbo].[ContactEmails].[Email] AS [Data],
	-- Buildings
	ISNULL([dbo].[BuildingContacts].[BuildingId], -1) AS [Id],
	[dbo].[BuildingContacts].[Type] AS [Label],
	[dbo].[Buildings].[Name] AS [Data],
	-- Customers
	ISNULL([dbo].[CustomerContacts].[CustomerId], -1) AS [Id],
	[dbo].[CustomerContacts].[Type] AS [Label],
	[dbo].[Customers].[Name] AS [Data]
FROM
	[dbo].[Contacts]
	LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].[Contacts].[Id] = [dbo].[ContactPhones].[ContactId]
	LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[Contacts].[Id] = [dbo].[ContactEmails].[ContactId]
	LEFT OUTER JOIN [dbo].[BuildingContacts] ON [dbo].[Contacts].[Id] = [dbo].[BuildingContacts].[ContactId]
	LEFT OUTER JOIN [dbo].[CustomerContacts] ON [dbo].[Contacts].[Id] = [dbo].[CustomerContacts].[ContactId]
	LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[BuildingContacts].[BuildingId] = [dbo].[Buildings].[ID]
	LEFT OUTER JOIN [dbo].[Customers] ON [dbo].[CustomerContacts].[CustomerId] = [dbo].[Customers].[ID]
WHERE
	[dbo].[Contacts].[ID] = @contactId

