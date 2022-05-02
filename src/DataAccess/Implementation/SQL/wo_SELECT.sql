declare @filter NVARCHAR(MAX);
declare @companyId int;
declare @pageSize int;
declare @pageNumber int;
declare @buildingId int;
declare @administratorId int;
declare @statusId int;


select @filter = '', @companyId = 5, @pageSize=100, @pageNumber=0;                

-- payload query
SELECT *, [Count] = COUNT(*) OVER() 
FROM (
	SELECT 
		[dbo].[WorkOrders].[ID],
		[dbo].[WorkOrders].[CreatedDate] AS DateSubmitted,
		ISNULL([dbo].[WorkOrders].[AdministratorId], 0) as AdministratorId,
		[dbo].[WorkOrders].[DueDate],

		CASE WHEN [dbo].[WorkOrders].[StatusId] = 0  AND [dbo].[WorkOrders].[Location] is null THEN [dbo].[WorkOrders].[FullAddress] ELSE [dbo].[WorkOrders].[Location] END as [Location],
		
		[dbo].[WorkOrders].[RequesterEmail] as RequesterEmail,
 
		CONCAT([dbo].[WorkOrders].[RequesterFirstName], [dbo].[WorkOrders].[RequesterLastName] ) as RequesterFullName,
			
		[dbo].[WorkOrders].[Number] as Number,
		[dbo].[WorkOrders].[RequesterId],
		ISNULL([dbo].[WorkOrders].[BuildingId], 0) as BuildingId,
		ISNULL([dbo].[WorkOrders].[StatusId],0 ) as StatusId,
		[dbo].[WorkOrders].[CompanyId],
		(SELECT COUNT(*) FROM WorkOrderNotes WHERE WorkOrderNotes.WorkOrderId = [dbo].[WorkOrders].[ID]) as NotesCount,
		(SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksCount,
		(SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID] AND WorkOrderTasks.IsComplete=1) as TasksDoneCount,
		(SELECT COUNT(*) FROM WorkOrderAttachments WHERE WorkOrderAttachments.WorkOrderId = [dbo].[WorkOrders].[ID]) AS AttachmentsCount,
		LEFT([dbo].[WorkOrders].[Description], 128) as [Description],	
				
		CONCAT(adminContact.[FirstName]+' ',adminContact.[MiddleName]+' ',adminContact.[LastName]+' ') as AdministratorFullName,
		CONCAT(assignedContact.[FirstName]+' ',assignedContact.[MiddleName]+' ',assignedContact.[LastName]+' ') as AssignedEmployeeFullName,
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
	LEFT OUTER JOIN [dbo].[Contacts] as requesterContact on requesterContact.[ID] = [dbo].[WorkOrders].[RequesterId]
	LEFT OUTER JOIN [dbo].[ContactEmails] as requesterEmail on requesterContact.ID = requesterEmail.ContactId and requesterEmail.[Default] = 1
	LEFT OUTER JOIN [dbo].[Employees] as [admin] on [admin].[ID] = [dbo].[WorkOrders].AdministratorId
	LEFT OUTER JOIN [dbo].[Employees] as assigned on assigned.[ID] = [dbo].[WorkOrders].AssignedEmployeeId
	LEFT OUTER JOIN [dbo].[Buildings] as bldg on bldg.[ID] = [dbo].[WorkOrders].[BuildingId]
	LEFT OUTER JOIN [dbo].[Contacts] as assignedContact on assignedContact.[ID] = assigned.ContactId
	LEFT OUTER JOIN [dbo].[Contacts] as adminContact on  adminContact.[ID] = [admin].ContactId


) payload 
WHERE CompanyId = @companyId AND
			AdministratorId = CASE WHEN @administratorId is null THEN AdministratorId ELSE @administratorId END AND
			StatusId = CASE WHEN @statusId is null THEN StatusId ELSE @statusId END AND
		    BuildingId = CASE WHEN @buildingId is null THEN BuildingId ELSE @buildingId END AND 
	      ISNULL([Description], '') + 
		  CAST(Number AS nvarchar(50)) +
		  RequesterFullName + 
		  AdministratorFullName + 
		  AssignedEmployeeFullName + 
		  [Location] + 
		  BuildingName		  
                LIKE '%' + ISNULL(@filter, '') + '%'

ORDER BY IsExpired DESC, DateSubmitted DESC, ID 
OFFSET @pageSize * @pageNumber ROWS
FETCH NEXT @pageSize ROWS ONLY