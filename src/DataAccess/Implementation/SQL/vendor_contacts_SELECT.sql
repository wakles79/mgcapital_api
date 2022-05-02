
declare @filter NVARCHAR(MAX);
declare @vendorId int;
declare @pageSize int;
declare @pageNumber int;

select @filter = '', @vendorId = 2, @pageSize=100, @pageNumber=0;    
-- payload query
SELECT *, [Count] = COUNT(*) OVER() FROM (
	SELECT 
	[dbo].[Contacts].[ID],
    [dbo].[Contacts].[CompanyId],
    [dbo].[Contacts].[DOB],
    [dbo].[Contacts].[Gender],
    [dbo].[Contacts].[Salutation],
    [dbo].[Contacts].[Title],
    [dbo].[Contacts].[Notes],
    [dbo].[Contacts].[Guid],
    [dbo].[ContactPhones].[Phone] as Phone,
    [dbo].[ContactPhones].[Ext] as Ext,
    [dbo].[ContactEmails].[Email] as Email,
	[dbo].[VendorContacts].[VendorId],
    CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[MiddleName]+' ',[dbo].[Contacts].[LastName]+' ') as FullName,
    CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress
	FROM [dbo].[VendorContacts]
    INNER JOIN  [dbo].[Contacts] ON  [dbo].[Contacts].[ID] = [dbo].[VendorContacts].[ContactId]
	LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].[Contacts].ID=[dbo].[ContactPhones].[ContactId] AND 
        ISNULL([dbo].[ContactPhones].[Default], 0) = 1
    LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[Contacts].ID = [dbo].[ContactEmails].[ContactId] AND 
        ISNULL([dbo].[ContactEmails].[Default], 0) = 1
    LEFT OUTER JOIN [dbo].[ContactAddresses] ON [dbo].[Contacts].ID = [dbo].[ContactAddresses].[ContactId] AND 
        ISNULL([dbo].[ContactAddresses].[Default], 0) = 1
    LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[ContactAddresses].[AddressId]
					) payload 
    WHERE VendorId= @vendorId AND
            ISNULL(FullName, '') + 
            ISNULL(Phone, '') + 
            ISNULL(Ext, '') +
            ISNULL(Email, '') +
            ISNULL(FullAddress, '')
                LIKE '%' + ISNULL(@filter, '') + '%'
ORDER BY ID
OFFSET @pageSize * @pageNumber ROWS
FETCH NEXT @pageSize ROWS ONLY;