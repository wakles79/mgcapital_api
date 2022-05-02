declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @isActive int;

select @filter = ''
		,@companyId = 5
		,@pageSize=100
		,@pageNumber=0
		,@isActive = 1;

SELECT *, [Count] = COUNT(*) OVER() FROM (

	SELECT C.ID,
	   C.ContractNumber,
	   B.Name as BuildingName,
	   CC.Name as CustomerFullName,
	   C.Status
       FROM [dbo].[Contracts] as C 
	   INNER JOIN [dbo].[Buildings] as B ON C.BuildingId = B.ID
	   INNER JOIN [dbo].[Customers] as CC ON C.CustomerId = CC.ID

	   WHERE C.CompanyId = @companyId AND
             ISNULL(C.ContractNumber, '') + ISNULL(B.Name, '') +
             ISNULL(CC.Name, '') LIKE '%' + ISNULL(@filter, '') + '%'         
                   
 ) payload 
                        
 ORDER BY ID
 OFFSET @pageSize * @pageNumber ROWS
 FETCH NEXT @pageSize ROWS ONLY;