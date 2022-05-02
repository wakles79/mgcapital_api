declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @id int;
declare @index int;
declare @maxIndex int;
declare @total int;

select @filter = '', @companyId = 5, @pageSize=50, @pageNumber=0;
--select @id = 940;

IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
BEGIN
    select @index =  @pageNumber;
END
ELSE
BEGIN
SELECT @index = [Index] - 1 FROM ( 
    SELECT 
		C.ID as ID, 
		C.CompanyId as CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY C.ContractNumber, C.ID ) as [Index]
    FROM [dbo].[Contracts] as C
    ) payload
WHERE ID = @id
END

SELECT @total = COUNT(*) FROM [dbo].[Contracts] WHERE [dbo].[Contracts].[CompanyId]= @companyId;

--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

--safety check
SELECT @index = ISNULL(@index, 0);

--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

SELECT 
	CC.ID as ID,
	CONCAT(CC.ContractNumber,' ', B.Name) as [Name]
FROM [dbo].[Contracts] as CC INNER JOIN [dbo].[Buildings] as B ON CC.BuildingId = B.ID
WHERE CC.CompanyId= @companyId AND
	ISNULL(CONCAT(CC.ContractNumber,' ', B.Name), '') 
		LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY Name, ID

OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY;
