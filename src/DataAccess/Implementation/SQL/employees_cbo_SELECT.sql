declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @employeeId int;
declare @index int;
declare @maxIndex int;
declare @total int;
declare @id int;
DECLARE @roleLevel INT;
select @filter = '', @companyId = 1, @pageSize=50, @pageNumber=0, @roleLevel=10;
select @id = 10;
IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
BEGIN
  select @index = @pageNumber;
END
ELSE
BEGIN
SELECT @index = [Index] - 1 FROM ( 
  SELECT 
    [dbo].[Employees].ID,
    [dbo].[Employees].CompanyId,
    R.[Level], 
    ROW_NUMBER() OVER (PARTITION BY [dbo].[Employees].CompanyId Order BY ISNULL([dbo].[Contacts].[FirstName], '') + ISNULL([dbo].[Contacts].[MiddleName], '') + ISNULL([dbo].[Contacts].[LastName], ''), [dbo].[Employees].ID) as [Index]
  FROM [dbo].[Employees]
  INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
  INNER JOIN [dbo].[Roles] AS R ON R.ID = Employees.RoleId
  ) payload
WHERE ID = @id AND [Level] = @roleLevel;
END
SELECT @total = COUNT(*) FROM [dbo].[Employees] WHERE [dbo].[Employees].[CompanyId]= @companyId;
--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);
--safety check
SELECT @index = ISNULL(@index, 0);
--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

DECLARE @mismatch AS int;
SELECT @mismatch = (SELECT 
						[dbo].[Employees].[ID] as ID
					FROM [dbo].[Employees]
						INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
					WHERE [dbo].[Employees].[ID] = @id AND [dbo].[Roles].[Level] = @roleLevel)


-- Always selecting exising employee (even though can be queried in last query, we use set union)
SELECT 
  [dbo].[Employees].[ID] as ID,
  CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[MiddleName]+' ',[dbo].[Contacts].[LastName]+' ', 
		CASE WHEN @mismatch is null THEN '[ROLE MISMATCH]' ELSE '' END
		) AS [Name],
  [dbo].[Roles].[Level]
FROM [dbo].[Employees]
INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
  WHERE [dbo].[Employees].[ID] = @id

UNION

-- Selects all employees that match companyId and roleLevel (in case @roleLevel is passed as parameter)
SELECT 
  [dbo].[Employees].[ID] as ID,
  CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[MiddleName]+' ',[dbo].[Contacts].[LastName]+' ') as Name,
  [dbo].[Roles].Level
FROM [dbo].[Employees]
INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
WHERE [dbo].[Employees].[CompanyId]= @companyId AND Roles.[Level] = @roleLevel AND
  ISNULL([dbo].[Contacts].[FirstName], '') +
  ISNULL([dbo].[Contacts].[MiddleName], '') +
  ISNULL([dbo].[Contacts].[LastName], '') 
    LIKE '%' + ISNULL(@filter, '') + '%' 
	AND [dbo].Employees.ID <> ISNULL(@mismatch, 0)   


ORDER BY [Name], [dbo].[Employees].ID
OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY