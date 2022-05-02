DECLARE @userEmail AS NVARCHAR(40) = 'axzesllc@gmail.com'
DECLARE @companyId AS INT = 5
DECLARE @buildingId AS INT = 539
DECLARE @supervisorType AS INT = 1
DECLARE @opersManagerType AS INT = 2

DECLARE @roleLevel AS INT;

SELECT 
	@roleLevel = [dbo].[Roles].Level 
FROM 
	[dbo].[Roles] 
    INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
WHERE 
	[dbo].[Employees].[CompanyId] = @companyId AND [dbo].[Employees].[Email] = @userEmail ;
	
SELECT 
	[dbo].[Buildings].[ID],
	[dbo].[Buildings].[Name],
	IIF(@roleLevel <= 20, [dbo].[Customers].[ID], -1) AS [CustomerId],
	IIF(@roleLevel <= 20, [dbo].[Customers].[Name], '') AS [CustomerName],
	[dbo].[Addresses].[FullAddress],
	[dbo].[Buildings].[EmergencyPhone],
	[dbo].[Buildings].[EmergencyPhoneExt],
	-- Operations Managers
	ISNULL([OMBE].[EmployeeId], -1) AS [Id],
	CONCAT([OMC].[FirstName], ' ', [OMC].[MiddleName], ' ', [OMC].[LastName]) AS [Data], 
	'Operations Manager' AS [Label],
	-- Supervisors
	ISNULL([SBE].[EmployeeId], -1) AS [Id],
	CONCAT([SC].[FirstName], ' ', [SC].[MiddleName], ' ', [SC].[LastName]) AS [Data], 
	'Supervisor' AS [Label],
	-- Contacts
	IIF(@roleLevel <= 20, ISNULL([BC].[Id], -1), -1) AS [Id],
	IIF(@roleLevel <= 20, CONCAT([BC].[FirstName], ' ', [BC].[MiddleName], ' ', [BC].[LastName]), '') AS [Data], 
	IIF(@roleLevel <= 20, [dbo].[BuildingContacts].[Type], '') AS [Label]
FROM
	[dbo].[Buildings]
	LEFT OUTER JOIN [dbo].[Contracts] ON [dbo].[Buildings].[Id] = [dbo].[Contracts].[BuildingId]
	LEFT OUTER JOIN [dbo].[Customers] ON [dbo].[Contracts].[CustomerId] = [dbo].[Customers].[Id]
	LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Buildings].[AddressId] = [dbo].[Addresses].[ID]

	LEFT OUTER JOIN [dbo].[BuildingEmployees] AS [SBE] ON [dbo].[Buildings].[Id] = [SBE].[BuildingId] AND [SBE].[Type] = @supervisorType
	LEFT OUTER JOIN [dbo].[Employees] AS [S] ON [SBE].[EmployeeId] = [S].[ID]
	LEFT OUTER JOIN [dbo].[Contacts] AS [SC] ON [S].[ContactId] = [SC].[ID]
	
	LEFT OUTER JOIN [dbo].[BuildingEmployees] AS [OMBE] ON [dbo].[Buildings].[Id] = [OMBE].[BuildingId] AND [OMBE].[Type] = @opersManagerType
	LEFT OUTER JOIN [dbo].[Employees] AS [OM] ON [OMBE].[EmployeeId] = [OM].[ID]
	LEFT OUTER JOIN [dbo].[Contacts] AS [OMC] ON [OM].[ContactId] = [OMC].[ID]

	LEFT OUTER JOIN [dbo].[BuildingContacts] ON [dbo].[Buildings].[ID] = [dbo].[BuildingContacts].[BuildingId]
	LEFT OUTER JOIN [dbo].[Contacts] AS [BC] ON [dbo].[BuildingContacts].[ContactId] = [BC].[ID]
WHERE
	[dbo].[Buildings].[ID] = @buildingId

