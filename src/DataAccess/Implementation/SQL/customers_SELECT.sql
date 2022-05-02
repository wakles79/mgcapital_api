
declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;

select @filter = '', @companyId = 2, @pageSize=100, @pageNumber=0;                

-- count query
SELECT COUNT(*) FROM (
	SELECT   
		[dbo].[Customers].[ID],
		[dbo].[Customers].[CompanyId],
		[dbo].[Customers].[Code], 
		[dbo].[Customers].[Name], 
		[dbo].[Customers].[StatusId], 
		[dbo].[Customers].[IsGenericAccount], 
		[dbo].[Customers].[Notes], 
		[dbo].[Customers].[MinimumProfitMargin], 
		[dbo].[Customers].[PONumberRequired], 
		[dbo].[Customers].[CreditLimit], 
		[dbo].[Customers].[CRHoldFlag], 
		[dbo].[Customers].[CreditTerms], 
		[dbo].[Customers].[ShowPricesOnShipper], 
		[dbo].[Customers].[InsuredUpTo], 
		[dbo].[Customers].[InsurerName], 
		[dbo].[Customers].[FinanceCharges], 
		[dbo].[Customers].[Guid], 
		[dbo].[Customers].[CreatedDate],
		[dbo].[CustomerPhones].[Phone] as Phone, 
		[dbo].[CustomerPhones].[Ext] as Ext,
		CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress
	FROM [dbo].[Customers] 
		LEFT OUTER JOIN [dbo].[CustomerPhones] ON [dbo].[Customers].ID=[dbo].[CustomerPhones].[CustomerId] AND ISNULL([dbo].[CustomerPhones].[Default], 0) = 1
		LEFT OUTER JOIN [dbo].[CustomerAddresses] ON [dbo].[Customers].ID = [dbo].[CustomerAddresses].[CustomerId] AND ISNULL([dbo].[CustomerAddresses].[Default], 0) = 1
		LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[CustomerAddresses].[AddressId]
	) payload
WHERE CompanyId=@companyId AND
        ISNULL(Name, '') + 
        ISNULL(Code, '') + 
        ISNULL(Phone, '') + 
        ISNULL(Ext, '') +
        ISNULL(FullAddress, '')
            LIKE '%' + ISNULL(@filter, '') + '%';
-- payload query
SELECT * FROM (
	SELECT   
		[dbo].[Customers].[ID],
		[dbo].[Customers].[CompanyId],
		[dbo].[Customers].[Code], 
		[dbo].[Customers].[Name], 
		[dbo].[Customers].[StatusId], 
		[dbo].[Customers].[IsGenericAccount], 
		[dbo].[Customers].[Notes], 
		[dbo].[Customers].[MinimumProfitMargin], 
		[dbo].[Customers].[PONumberRequired], 
		[dbo].[Customers].[CreditLimit], 
		[dbo].[Customers].[CRHoldFlag], 
		[dbo].[Customers].[CreditTerms], 
		[dbo].[Customers].[ShowPricesOnShipper], 
		[dbo].[Customers].[InsuredUpTo], 
		[dbo].[Customers].[InsurerName], 
		[dbo].[Customers].[FinanceCharges], 
		[dbo].[Customers].[Guid], 
		[dbo].[CustomerPhones].[Phone] as Phone, 
		[dbo].[CustomerPhones].[Ext] as Ext,
		CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress
	FROM [dbo].[Customers] 
		LEFT OUTER JOIN [dbo].[CustomerPhones] ON [dbo].[Customers].ID=[dbo].[CustomerPhones].[CustomerId] AND ISNULL([dbo].[CustomerPhones].[Default], 0) = 1
		LEFT OUTER JOIN [dbo].[CustomerAddresses] ON [dbo].[Customers].ID = [dbo].[CustomerAddresses].[CustomerId] AND ISNULL([dbo].[CustomerAddresses].[Default], 0) = 1
		LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[CustomerAddresses].[AddressId]
	) payload
WHERE CompanyId=@companyId AND
        ISNULL(Name, '') + 
        ISNULL(Code, '') + 
        ISNULL(Phone, '') + 
        ISNULL(Ext, '') +
        ISNULL(FullAddress, '')
            LIKE '%' + ISNULL(@filter, '') + '%'

ORDER BY CreatedDate DESC, ID
OFFSET @pageSize * @pageNumber ROWS
FETCH NEXT @pageSize ROWS ONLY