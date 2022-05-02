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
		[dbo].[CleaningReports].ID as ID, 
		[dbo].[CleaningReports].[CompanyId] as CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[CleaningReports].[Number], [dbo].[CleaningReports].ID ) as [Index]
    FROM [dbo].[CleaningReports]
    ) payload
WHERE ID = @id;
END

SELECT @total = COUNT(*) FROM [dbo].[CleaningReports] WHERE [dbo].[CleaningReports].[CompanyId]= @companyId;

--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

--safety check
SELECT @index = ISNULL(@index, 0);

--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

SELECT 
	[dbo].[CleaningReports].[ID],
	[dbo].[CleaningReports].[Number],
	[dbo].[CleaningReports].[ContactId] as CustomerContactId

FROM [dbo].[CleaningReports]
WHERE [dbo].[CleaningReports].[CompanyId]= @companyId AND
	ISNULL([dbo].[CleaningReports].[Number], '') 
		LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY Number, ID

OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY;