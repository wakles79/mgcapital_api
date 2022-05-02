declare @email nvarchar(max);
declare @companyId int;
declare @workOrderId int;
declare @buildingId int;

SELECT @workOrderId = 401;

SELECT @companyId = 1, @email = 'axzesllc@gmail.com';

SELECT 
	WO.ID,
	WO.Number AS WONumber,
	WO.Description,
	B.Name AS BuildingName,
	CONCAT(EC.FirstName, ' ', EC.LastName) AS AssignedFullName,
	CONCAT(RC.FirstName, ' ', RC.LastName) AS RequesterFullName,
	RCEmail.Email AS RequesterEmail,
	ISNULL((SELECT 
		TOP 1 CONCAT(EC.FirstName, ' ', EC.LastName) AS FullName
		FROM Employees AS E
			INNER JOIN Contacts AS EC ON EC.ID = E.ContactId
		WHERE E.Email = @email AND E.CompanyId = @companyId), '') AS EmployeeWhoClosedWO

FROM WorkOrders AS WO
	LEFT JOIN Buildings AS B ON WO.BuildingId = B.ID
	LEFT JOIN Employees AS E ON WO.AssignedEmployeeId = E.ID
	LEFT JOIN Contacts AS EC ON EC.ID = E.ContactId
	LEFT JOIN Contacts AS RC ON RC.ID = WO.RequesterId
	LEFT JOIN ContactEmails AS RCEmail ON RC.ID = RCEmail.ContactId
WHERE WO.ID = @workOrderId
