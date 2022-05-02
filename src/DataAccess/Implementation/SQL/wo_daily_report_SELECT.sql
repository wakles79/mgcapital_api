declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @buildingId int;
declare @administratorId int;
declare @statusId int;
declare @employeeId  int;
declare @dateFrom Date;
declare @dateTo Date;

select @filter = '', @companyId = 1, @pageSize=100, @pageNumber=0, @employeeId = 20;          
   

-- payload query
SELECT *, [Count] = COUNT(*) OVER() 
FROM (
	SELECT 
		[dbo].[WorkOrders].[ID],
        [dbo].[WorkOrders].[Guid],
		[dbo].[WorkOrders].[CreatedDate] AS DateSubmitted,
		ISNULL([dbo].[WorkOrders].[AdministratorId], 0) as AdministratorId,
		[dbo].[WorkOrders].[DueDate],
		CASE WHEN [dbo].[WorkOrders].[StatusId] = 0  AND [dbo].[WorkOrders].[Location] is null THEN [dbo].[WorkOrders].[FullAddress] ELSE [dbo].[WorkOrders].[Location] END as [Location],
		[dbo].[WorkOrders].[RequesterEmail] as RequesterEmail,
		[dbo].[WorkOrders].[RequesterFullName],
		[dbo].[WorkOrders].[Number] as Number,
		ISNULL([dbo].[WorkOrders].[BuildingId], 0) as BuildingId,
		ISNULL([dbo].[WorkOrders].[StatusId],0 ) as StatusId,
		[dbo].[WorkOrders].[CompanyId],
		(SELECT COUNT(*) FROM WorkOrderNotes WHERE WorkOrderNotes.WorkOrderId = [dbo].[WorkOrders].[ID]) as NotesCount,
		(SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksCount,
		(SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID] AND WorkOrderTasks.IsComplete=1) as TasksDoneCount,
		(SELECT COUNT(*) FROM WorkOrderAttachments WHERE WorkOrderAttachments.WorkOrderId = [dbo].[WorkOrders].[ID]) as AttachmentsCount,
		LEFT([dbo].[WorkOrders].[Description], 128) as [Description],
		CONCAT(operationsManagerContact.[FirstName]+' ',operationsManagerContact.[MiddleName]+' ',operationsManagerContact.[LastName]+' ') as OperationsManagerFullName,
		ISNULL(bldg.[Name], '') as BuildingName,
		[dbo].[WorkOrders].[IsExpired],

        -- CLONING FIELDS
        [dbo].[WorkOrders].[OriginWorkOrderId],
	    CASE 
			WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		    ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[WorkOrders].[Number]) 
	    END AS [ClonePath]

	    FROM [dbo].[WorkOrders]
		LEFT OUTER JOIN [dbo].[Contacts] as customerContact on customerContact.[ID] = [dbo].[WorkOrders].[CustomerContactId]
		LEFT OUTER JOIN [dbo].[Buildings] as bldg on bldg.[ID] = [dbo].[WorkOrders].[BuildingId]
		LEFT OUTER JOIN [dbo].WorkOrderEmployees as [woEmployees] on woEmployees.WorkOrderId = [dbo].[WorkOrders].ID
		LEFT OUTER JOIN [dbo].[Employees] as [employee] on [employee].[ID] = [woEmployees].EmployeeId
		LEFT OUTER JOIN [dbo].[Contacts] as operationsManagerContact on  operationsManagerContact.[ID] = [employee].ContactId
		
		WHERE [woEmployees].[Type] = 2	AND 
			  [woEmployees].EmployeeId = @employeeId 
			  --AND CAST([dbo].[WorkOrders].[CreatedDate] AS DATE) >= @dateFrom 
			  --AND CAST([dbo].[WorkOrders].[CreatedDate] AS DATE) <= @dateTo	

) payload 
WHERE CompanyId = @companyId AND
        ISNULL([Description], '') + 
        CAST(Number AS nvarchar(50)) +
        RequesterFullName + 
        [Location] + 
        BuildingName
			LIKE '%' + ISNULL(@filter, '') + '%'
        ORDER BY IsExpired DESC, DateSubmitted DESC, ID 
        OFFSET @pageSize * @pageNumber ROWS
        FETCH NEXT @pageSize ROWS ONLY


--select * from WorkOrderEmployees