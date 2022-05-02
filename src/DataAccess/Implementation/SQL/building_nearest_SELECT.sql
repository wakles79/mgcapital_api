declare @latitude float;
declare @longitude float;
declare @epsilon float;
declare @companyId int;

select @latitude = 35.8263232, @longitude = -78.7957315, @companyId = 1, @epsilon = 1e-2;

SELECT b.[ID]
      ,b.[AddressId]
      ,b.[CompanyId]
      ,b.[CreatedBy]
      ,b.[CreatedDate]
      ,b.[Guid]
      ,b.[Name]
      ,b.[UpdatedBy]
      ,b.[UpdatedDate]
      ,b.[IsActive]
      ,b.[OperationsManagerId]
      ,b.[SupervisorId]
      ,b.[IsAvailable]
  FROM [dbo].[Buildings] as b
		INNER JOIN [dbo].[Addresses] as a ON b.AddressId = a.ID
  WHERE SQRT(POWER(a.Latitude - @latitude, 2) + POWER(a.Longitude - @longitude, 2)) <= @epsilon AND
		b.CompanyId = @companyId
  ORDER BY SQRT(POWER(a.Latitude - @latitude, 2) + POWER(a.Longitude - @longitude, 2))
  