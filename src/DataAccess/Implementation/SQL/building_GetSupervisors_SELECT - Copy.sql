--DECLARE @buildingId AS INT = 381

SELECT 
    [dbo].[Employees].[Id],
    [dbo].[Employees].[Email],
    CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name],
    [dbo].[Roles].[Name] AS [RoleName],
    1 AS [Type]
FROM
    [dbo].[Employees]
    LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[Id]
    LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[Id]
    INNER JOIN [dbo].[BuildingEmployees] ON [dbo].[Employees].[Id] = [dbo].[BuildingEmployees].[EmployeeId]
WHERE
    [dbo].[BuildingEmployees].[BuildingId] = @buildingId