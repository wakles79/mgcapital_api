				DECLARE @companyId int;
				DECLARE @buildingId int;
				DECLARE @filter NVARCHAR(MAX);
				SELECT @companyId = 1, @filter = 'Mar';


                SELECT * FROM (

                    SELECT E.ID, 
                          CONCAT(C.FirstName, ' ', C.MiddleName, ' ', C.LastName) AS [Name],
                          R.Name AS RoleName,
                          0 AS Selected
                    FROM Employees AS E 
                        INNER JOIN Contacts AS C ON E.ContactId = C.ID
                        INNER JOIN BuildingEmployees AS BE ON BE.EmployeeId = E.ID AND BE.Type = 1
                        INNER JOIN Buildings AS B ON BE.BuildingId = B.ID
                        INNER JOIN Roles AS R ON R.ID = E.RoleId
                    WHERE B.ID = @buildingId

                    UNION 

                    SELECT E.ID, 
                          CONCAT(C.FirstName, ' ', C.MiddleName, ' ', C.LastName) AS [Name],
                          R.Name AS RoleName,
                          1 AS Selected
                    FROM Employees AS E 
                        INNER JOIN Contacts AS C ON E.ContactId = C.ID
                        INNER JOIN Roles AS R ON E.RoleId = R.ID
                    WHERE E.CompanyId = @companyId AND R.[Level] = 40

                    ) Q
					WHERE Q.Name LIKE '%' + @filter + '%'
                    ORDER BY Q.Selected, Q.Name, Q.ID