declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @id int;
declare @index int;
declare @maxIndex int;
declare @total int;
select @filter = '', @companyId = 1, @pageSize=20, @pageNumber=0, @id= 363;
--select @id = 940;
                IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
                BEGIN
                    select @index =  @pageNumber;
                END
                ELSE
                BEGIN
                SELECT @index = [Index] - 1 FROM ( 
                    SELECT 
                        B1.ID as ID, 
                        B1.CompanyId as CompanyId,
                        ROW_NUMBER() OVER (PARTITION BY CompanyId Order By B1.Name, B1.ID ) as [Index]
                    FROM [dbo].[Buildings] as B1
						INNER JOIN [dbo].Addresses as A ON B1.AddressId = A.ID 
						WHERE B1.IsActive = 1 AND (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = B1.Id AND [Type] = 2 ) > 0
                    ) payload
                WHERE ID = @id 
				AND 
                (select case when  (SELECT top(1) [dbo].[Buildings].ID
                                    FROM [dbo].[Contracts] INNER JOIN [dbo].[Buildings] ON [dbo].[Contracts].BuildingId = [dbo].[Buildings].ID
                                    WHERE 
                                        [dbo].[Contracts].Status = 1 AND [dbo].[Buildings].IsActive = 1 AND 
                                        (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = [dbo].[Buildings].Id AND [Type] = 2 ) > 0
                                    ) is null then 0 else 1 end) = 1 ;
                END

                SELECT @total = COUNT(*) FROM [dbo].[Buildings] as B2 WHERE B2.CompanyId= @companyId;

                --max(0, @total-@pageSize)
                SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                --safety check
                SELECT @index = ISNULL(@index, 0);

                --min(@index, @maxIndex)
                SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                SELECT 
                    B3.ID as ID,
	                B3.Name as Name,
					A.FullAddress as FullAddress
                        FROM [dbo].[Buildings] as B3 INNER JOIN [dbo].Addresses as A ON B3.AddressId = A.ID
                        WHERE B3.[CompanyId]= @companyId AND B3.[IsActive] = 1 AND
                            (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = B3.Id AND [Type] = 2 ) > 0 AND
                    ISNULL(B3.Name, '') +
                    ISNULL(A.FullAddress, '')
                        LIKE '%' + ISNULL(@filter, '') + '%' 
                ORDER BY Name, ID

                OFFSET @index ROWS
                FETCH NEXT @pageSize ROWS ONLY 