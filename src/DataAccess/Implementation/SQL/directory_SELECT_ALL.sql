--DECLARE @companyId AS INT = 5;
--DECLARE @userEmail AS NVARCHAR(80) = 'axzesllc@gmail.com';
DECLARE @roleLevel AS INT;

SELECT 
	@roleLevel = [dbo].[Roles].Level 
FROM 
	[dbo].[Roles] 
    INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
WHERE 
	[dbo].[Employees].[CompanyId] = @companyId AND [dbo].[Employees].[Email] = @userEmail ;


SELECT * FROM (
	SELECT 
		[dbo].[Employees].[ID], 
		CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name], 
		'1' AS [Type]
	FROM [dbo].[Employees]
		LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
	WHERE [dbo].[Employees].[CompanyId] = @companyId

	UNION ALL

	SELECT 
		[dbo].[Buildings].[Id], 
		[dbo].[Buildings].[Name], 
		'2' AS [Type] 
	FROM [dbo].[Buildings] 
	WHERE [dbo].[Buildings].[IsActive] = 1 AND [dbo].[Buildings].[CompanyId] = @companyId

	UNION ALL
	
	SELECT 
		[dbo].[Customers].[ID], 
		[dbo].[Customers].[Name], 
		'4' AS [Type]
	FROM [dbo].[Customers]
	WHERE @roleLevel <= 20 AND [dbo].[Customers].[CompanyId] = @companyId 
) AS q
ORDER BY q.[Name], q.[Type] DESC 