declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @id int;
declare @index int;
declare @maxIndex int;
declare @total int;

select @filter = '', @companyId = 1, @pageSize=50, @pageNumber=0;
--select @id = 2;

IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
BEGIN
    select @index =  @pageNumber;
END
ELSE
BEGIN
SELECT @index = [Index] - 1 FROM ( 
    SELECT 
		[dbo].[Services].ID as ID, 
		[dbo].[Services].[CompanyId] as CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[Services].[Name], [dbo].[Services].ID ) as [Index]
    FROM [dbo].[Services]
    ) payload
WHERE ID = @id;
END

SELECT @total = COUNT(*) FROM [dbo].[Services] WHERE [dbo].[Services].[CompanyId]= @companyId;

--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

--safety check
SELECT @index = ISNULL(@index, 0);

--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

SELECT 
	[dbo].[Services].[ID],
	[dbo].[Services].[Name],
	[dbo].[Services].[UnitFactor],
	[dbo].[Services].[UnitPrice],
	[dbo].[Services].[MinPrice]
FROM [dbo].[Services]
WHERE [dbo].[Services].[CompanyId]= @companyId AND
	ISNULL([dbo].[Services].[Name], '') 
		LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY Name, ID

OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY;
