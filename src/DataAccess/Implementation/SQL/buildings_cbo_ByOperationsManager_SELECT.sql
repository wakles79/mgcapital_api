 declare @companyId int = 1;
 declare @filter nvarchar(max);
 declare @currentEmployeeId int = 14;
 declare @pageSize int = 100;
 declare @pageNumber int = 0;

SELECT
	B.Id,	
	B.Name,
	A.FullAddress,
	IsSupervisor = 
	    CASE WHEN BE.Type = 1 THEN 1
	    ELSE  0 END ,

    IsTemporaryOperationsManager = 
	    CASE WHEN BE.Type = 4 THEN 1
	    ELSE 0 END,

	OperationsManagerFullName = CONCAT_WS(' ', C.FirstName, C.MiddleName, C.LastName)

FROM
	Buildings AS B
	INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID 
				AND ( (BE.EmployeeId <> @currentEmployeeId AND BE.Type = 2) 
                OR (BE.EmployeeId = @currentEmployeeId AND BE.Type IN (1 , 4)) ) 
	INNER JOIN BuildingEmployees AS BEE ON BEE.BuildingId = B.ID
	INNER JOIN Employees  AS E ON E.ID = BEE.EmployeeId AND BEE.Type = 2
	LEFT OUTER JOIN Contacts AS C ON C.ID = E.ContactId
	INNER JOIN Addresses AS A ON B.AddressId= A.ID

WHERE B.CompanyId = @companyId	
        AND B.isActive = 1  
	    AND ( BE.EmployeeId = @currentEmployeeId OR
            ( BE.EmployeeId  <> @currentEmployeeId AND B.[ID] NOT IN (SELECT BuildingId FROM BuildingEmployees 
                                                    WHERE EmployeeId = @currentEmployeeId 
                                                    AND type IN (1 , 4) ) ) )
                                                         
	    AND ISNULL(B.Name, '') + ISNULL(A.FullAddress, '') 
		LIKE '%' + ISNULL(@filter, '') + '%'	
ORDER BY IsSupervisor DESC, IsTemporaryOperationsManager DESC, B.[Name]
OFFSET @pageSize * @pageNumber ROWS
FETCH NEXT @pageSize ROWS ONLY