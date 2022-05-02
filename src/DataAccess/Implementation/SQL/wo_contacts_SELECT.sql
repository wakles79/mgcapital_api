declare @buildingId int;
declare @requesterId int;


-- Office Staff
SELECT 
    CONCAT(eContact.FirstName, ' ', eContact.LastName) AS FullName,
    E.Email AS Email,
    'Office Staff' AS Type,
    eContact.SendNotifications AS SendNotifications
FROM Employees AS E
	INNER JOIN Roles AS R ON E.RoleId = R.ID
    INNER JOIN Contacts AS eContact ON eContact.ID = E.ContactId
WHERE E.CompanyId = @companyId AND R.[Level] <= 20 -- Only Office Staff or higher
		--AND eContact.SendNotifications = 1

UNION

-- Building Owner
SELECT 
    CONCAT(cContact.FirstName, ' ', cContact.LastName) AS FullName,
    cContactEmail.Email AS Email,
    'Building Owner' AS Type,
    cContact.SendNotifications AS SendNotifications
FROM Buildings AS B 
    INNER JOIN Contracts AS contr ON contr.BuildingId = B.ID AND contr.[Status] = 1
    INNER JOIN Customers AS cust ON contr.CustomerId = cust.ID
    INNER JOIN CustomerContacts AS CC ON cust.ID = CC.CustomerId AND CC.[Default] = 1
    INNER JOIN Contacts AS cContact ON cContact.ID = CC.ContactId
    INNER JOIN ContactEmails AS cContactEmail ON cContactEmail.ContactId = cContact.ID
WHERE B.ID = @buildingId --AND cContact.SendNotifications = 1

UNION

-- Operations Manager
SELECT 
    CONCAT(omContact.FirstName, ' ', omContact.LastName) AS FullName,
    E.Email AS Email,
    'Operations Manager' AS Type,
    omContact.SendNotifications AS SendNotifications
FROM Buildings AS B 
    INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID AND BE.Type = {0}
    INNER JOIN Employees AS E ON BE.EmployeeId = E.ID 
    INNER JOIN Contacts AS omContact ON omContact.ID = E.ContactId
WHERE B.ID = @buildingId --AND omContact.SendNotifications = 1

UNION

-- Supervisor
SELECT 
    CONCAT(sContact.FirstName, ' ', sContact.LastName) AS FullName,
    E.Email AS Email,
    'Supervisor' AS Type,
    sContact.SendNotifications AS SendNotifications
FROM Buildings AS B 
	INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID AND BE.Type = {1}
    INNER JOIN Employees AS E ON BE.EmployeeId = E.ID 
    INNER JOIN Contacts AS sContact ON sContact.ID = E.ContactId
WHERE B.ID = @buildingId --AND sContact.SendNotifications = 1

UNION
			
-- Property Manager
SELECT 
    CONCAT(bContact.FirstName, ' ', bContact.LastName) AS FullName,
    bContactEmail.Email AS Email,
    'Property Manager' AS Type,
    bContact.SendNotifications AS SendNotifications
FROM Buildings AS B 
    INNER JOIN BuildingContacts AS BC ON BC.BuildingId = B.ID AND BC.Type = {2} 
    INNER JOIN Contacts AS bContact ON bContact.ID = BC.ContactId
    INNER JOIN ContactEmails AS bContactEmail ON bContactEmail.ContactId = bContact.ID
WHERE B.ID = @buildingId --AND bContact.SendNotifications = 1
