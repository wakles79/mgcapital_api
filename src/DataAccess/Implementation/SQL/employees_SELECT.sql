declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @isActive int;
declare @roleLevel int;

select @filter = ''
		,@companyId = 5
		,@pageSize=100
		,@pageNumber=0
		,@isActive = 1
		,@roleLevel = null;
		                    
					SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
						    E.ID,
							E.Guid,
							CONCAT(C.FirstName, C.MiddleName, C.LastName) as FullName,
							E.Email,
							R.Name as RoleName,	
							R.Level as RoleLevel,
							(SELECT Phone
							 from [dbo].ContactPhones  
							 WHERE [Default] = 1 and ContactId = e.ContactId
							) as Phone,
							E.CreatedDate,
							E.EmployeeStatusId,
							D.Name	as DepartmentName

	                    FROM [dbo].[Employees] as E 
						INNER JOIN [dbo].Contacts as C ON E.ContactId = C.ID	
						INNER JOIN [dbo].Roles as R ON E.RoleId = R.ID
						LEFT JOIN [dbo].ContactPhones as CP ON C.ID = CP.ContactId
						LEFT JOIN [dbo].Departments as D ON D.ID = E.DepartmentId		

					   WHERE E.[CompanyId] = @companyId AND
					   R.Level = CASE WHEN @roleLevel IS NULL THEN R.Level ELSE @roleLevel END AND
					   CONCAT(C.FirstName, C.MiddleName, C.LastName) +
					   Email +
					   R.Name +
					   ISNULL(Phone,'') LIKE '%' + ISNULL(@filter, '') + '%'
						 	
					 ) payload 
                        
                    ORDER BY FullName ASC, ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;