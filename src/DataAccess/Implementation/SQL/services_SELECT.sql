declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @isActive int;

select @filter = ''
		,@companyId = 1
		,@pageSize=100
		,@pageNumber=0

SELECT *, [Count] = COUNT (*) OVER() FROM (
  SELECT 
 S.[ID],      
 S.[MinPrice],
 S.[Name],
 S.[UnitFactor],
 S.[UnitPrice]
 FROM [dbo].[Services] as S 
 WHERE S.CompanyId = @CompanyId AND
	   ISNULL(S.Name, '') LIKE '%' + ISNULL(@filter, '') + '%'
  ) payload 
  ORDER BY ID
  OFFSET @pageSize * @pageNumber ROWS
  FETCH NEXT @pageSize ROWS ONLY;