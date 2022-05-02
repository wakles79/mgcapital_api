--DECLARE @EmployeeId AS INT = 18

SELECT 
	[dbo].[Employees].[ID], 
	CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name],
	[dbo].[Employees].[Email] AS [UserEmail],
	[dbo].[Departments].[Name] AS [Department],
	[dbo].[Roles].[Level] AS [Role],
	1 AS [Type],
	[dbo].[Contacts].[DOB] AS [Birthday],
	[dbo].[Contacts].[Notes],
	[dbo].[Contacts].[SendNotifications],
	-- Phones
	ISNULL([dbo].[ContactPhones].[ID], -1) AS [Id],
	[dbo].[ContactPhones].[Phone] AS [Data],
	[dbo].[ContactPhones].[Type] AS [Label],
	-- Emails
	ISNULL([dbo].[ContactEmails].[ID], -1) AS [Id],
	[dbo].[ContactEmails].[Email] AS [Data],
	[dbo].[ContactEmails].[Type] AS [Label]
FROM 
	[dbo].[Employees]
	LEFT OUTER JOIN [dbo].[Departments] ON [dbo].[Employees].[DepartmentId] = [dbo].[Departments].[ID]
	LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[ID]
	LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
	LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].ContactPhones.[ContactId] = [dbo].[Contacts].[ID]
	LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[ContactEmails].[ContactId] = [dbo].[Contacts].[ID]
WHERE
	[dbo].[Employees].[ID] = @EmployeeId 

