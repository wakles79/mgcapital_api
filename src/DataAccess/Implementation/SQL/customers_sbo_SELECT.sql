declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @id int;
declare @index int;
declare @maxIndex int;
declare @total int;

select @filter = '', @companyId = 2, @pageSize=50, @pageNumber=0;
--select @id = 3231;
--select @id=4588;

IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
BEGIN
    select @index =  @pageNumber;
END
ELSE
BEGIN
SELECT @index = [Index] - 1 FROM ( 
    SELECT 
		[dbo].[Customers].ID as ID, 
		[dbo].[Customers].[CompanyId] as CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[Customers].[Name], [dbo].[Customers].ID ) as [Index]
    FROM [dbo].[Customers]
    ) payload
WHERE ID = @id;
END

SELECT @total = COUNT(*) FROM [dbo].[Customers] WHERE [dbo].[Customers].[CompanyId]= @companyId;

--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

--safety check
SELECT @index = ISNULL(@index, 0);

--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

SELECT 
	[dbo].[Customers].[ID] as ID,
	[dbo].[Customers].[Name] as [Name]
FROM [dbo].[Customers]
WHERE [dbo].[Customers].[CompanyId]= @companyId AND
	ISNULL([dbo].[Customers].[Name], '') 
		LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY Name, ID

OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY;
