--DECLARE @useremail AS NVARCHAR(120) = 'axzesllc@gmail.com'

SELECT 
    [dbo].[Employees].[Id] AS [EmployeeId],
    [dbo].[Employees].[Guid] AS [EmployeeGuid],
    [dbo].[Employees].[Email] AS [EmployeeEmail],
    [dbo].[Roles].[Level] AS [RoleLevel],
    CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName],
    [dbo].[Companies].[Id] AS [CompanyId],
    [dbo].[Companies].[Name] AS [CompanyName]
FROM
    [dbo].[Employees]
	INNER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[ID]
    INNER JOIN [dbo].[Companies] ON [dbo].[Employees].[CompanyId] = [dbo].[Companies].[Id]
    INNER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[Id]
WHERE
    [dbo].[Employees].[Email] = @useremail 

