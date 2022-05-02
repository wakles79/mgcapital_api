declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @id int;
declare @index int;
declare @maxIndex int;
declare @total int;

select @filter = 'test', @companyId = 1, @pageSize=9999, @pageNumber=0;
--select @id = 3231;
--select @id=4588;

IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
BEGIN
    select @index =  @pageNumber;
END
ELSE
BEGIN
SELECT @index = [Index] - 1 FROM ( 
    SELECT 
		[dbo].[Contacts].ID as ID, 
		[dbo].[Contacts].[CompanyId] as CompanyId,
        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY ISNULL(CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[LastName]), ''), [dbo].[Contacts].ID ) as [Index]
    FROM [dbo].[Contacts]
    ) payload
WHERE ID = @id;
END

SELECT @total = COUNT(*) FROM [dbo].[Contacts] WHERE [dbo].[Contacts].[CompanyId]= @companyId;

--max(0, @total-@pageSize)
SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

--safety check
SELECT @index = ISNULL(@index, 0);

--min(@index, @maxIndex)
SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

SELECT * FROM (SELECT 
	C.[ID] as ID,
	C.CompanyId,
	CONCAT(C.[FirstName], ' ', C.[LastName]) as [Name],
	-- First Matching Phone
	ISNULL((SELECT TOP 1 CP.Phone FROM [dbo].[ContactPhones] AS CP WHERE CP.ContactId = C.ID), '') AS Phone,
	-- First Matching Email
	ISNULL((SELECT TOP 1 CE.Email FROM [dbo].[ContactEmails] AS CE WHERE CE.ContactId = C.ID), '') AS Email,
    -- First Matching Address
	ISNULL((SELECT TOP 1 CONCAT(A.AddressLine1, ' ', A.City, ' ', A.State, ' ', A.ZipCode) FROM [dbo].[ContactAddresses] AS CA INNER JOIN [dbo].[Addresses] AS A ON A.ID = CA.AddressId WHERE CA.ContactId = C.ID), '') AS FullAddress
FROM [dbo].[Contacts] AS C) payload 

WHERE CompanyId= @companyId AND
	ISNULL(CONCAT(Name, Phone, Email, FullAddress), '') 
		LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY Name, ID

OFFSET @index ROWS
FETCH NEXT @pageSize ROWS ONLY;
