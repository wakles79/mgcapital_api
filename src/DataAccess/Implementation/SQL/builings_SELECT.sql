declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @isActive int;
declare @isAvailable int;

select @filter = ''
		,@companyId = 1
		,@pageSize=9999
		,@pageNumber=0
		,@isActive = 1;
		                    
					SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
                        B.ID,
						B.[Name],
						B.[IsActive],
						B.[CompanyId],
                        B.[CreatedDate],
                        B.[EmergencyPhone],
						A.[FullAddress],
						OMC.FirstName + ' ' + OMC.LastName as OperationsManagerFullName,

						-- DUPLICATE QUERIES
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = 2 ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsComplete,

						-- TODO: For the sake of optimization, re-use the previous query
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND Bb.IsActive = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = 2 ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsAvailable

	                    FROM [dbo].[Buildings] as B
							INNER JOIN  [dbo].[Addresses] as A ON B.AddressId = A.ID
							LEFT OUTER JOIN [dbo].[Employees] as E on B.OperationsManagerId = E.ID
							LEFT OUTER JOIN [dbo].[Contacts] as OMC on E.ContactId = OMC.ID

                       WHERE B.[CompanyId] = @companyId AND
                        B.[IsActive] = CASE WHEN ISNULL(@isActive, -1) = -1 THEN B.[IsActive] ELSE @isActive END AND
                         ISNULL(B.[Name], '') + ISNULL((OMC.FirstName + ' ' + OMC.LastName), '') +
                         ISNULL(A.[FullAddress], '') +
                         ISNULL(B.[Name], '') LIKE '%' + ISNULL(@filter, '') + '%'
					) payload                         
                       WHERE IsAvailable = CASE WHEN ISNULL(@isAvailable, -1) = -1 THEN IsAvailable ELSE @isAvailable END
                    ORDER BY IsAvailable DESC, ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;

