				declare @companyId int;
				declare @pageNumber int;
				declare @pageSize int;
				declare @contactId int;
				declare @filter nvarchar(max);
				declare @id int;
				SELECT @companyId = 1, @contactId = 224, @pageSize = 50, @pageNumber = 0, @filter = '';

				SELECT @id=672;

				
				declare @index int;
                declare @maxIndex int;
                declare @total int;

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
                        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY B1.Name, B1.ID ) as [Index]
                    FROM [dbo].[Buildings] as B1 
						INNER JOIN [dbo].Addresses as A ON B1.AddressId = A.ID
						LEFT OUTER JOIN [dbo].[BuildingContacts] as C ON B1.ID = C.[BuildingId]
						WHERE C.[ContactId] = CASE WHEN ISNULL(@contactId, 0) = 0 THEN C.[ContactId] ELSE @contactId END
                    ) payload
                WHERE ID = @id
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
						LEFT OUTER JOIN [dbo].[BuildingContacts] as C ON B3.ID = C.[BuildingId]
                        WHERE B3.[CompanyId]= @companyId AND C.[ContactId] = CASE WHEN ISNULL(@contactId, 0) = 0 THEN C.[ContactId] ELSE @contactId END AND
                    ISNULL(B3.Name, '') +
                    ISNULL(A.FullAddress, '')
                        LIKE '%' + ISNULL(@filter, '') + '%'  
                ORDER BY Name, ID

                OFFSET @index ROWS
                FETCH NEXT @pageSize ROWS ONLY;