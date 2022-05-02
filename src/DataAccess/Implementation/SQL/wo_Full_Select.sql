DECLARE @workOrderId AS INT = 547
--DECLARE @workOrderGuid AS UNIQUEIDENTIFIER = '1653FEC7-4A38-495E-9EFB-648DBF00D03F'

SELECT 
	-- WO BASE VIEW MDDEL FIELDS
	[dbo].[WorkOrders].[ID],
	[dbo].[WorkOrders].[BuildingId],
	[dbo].[WorkOrders].[Location],
	[dbo].[WorkOrders].[AdministratorId],
	[dbo].[WorkOrders].[Priority],
	[dbo].[WorkOrders].[SendRequesterNotifications],
	[dbo].[WorkOrders].[SendPropertyManagersNotifications],
	[dbo].[WorkOrders].[StatusId],
	[dbo].[WorkOrders].[Number],
	[dbo].[WorkOrders].[Description],
	[dbo].[WorkOrders].[DueDate],
	[dbo].[WorkOrders].[Type],
	[dbo].[WorkOrders].[WorkOrderSourceId],
	[dbo].[WorkOrders].[BillingName],
	[dbo].[WorkOrders].[BillingEmail],
	[dbo].[WorkOrders].[BillingNote],
    [dbo].[WorkOrders].[ClosingNotes],
    [dbo].[WorkOrders].[OriginWorkOrderId],
	CASE 
		WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[WorkOrders].[Number]) 
	END AS [ClonePath],

	-- WO REQUESTER VIEW MODEL FIELDS
	[dbo].[WorkOrders].[FullAddress],
	[dbo].[WorkOrders].[RequesterFullName],
	[dbo].[WorkOrders].[RequesterEmail],

	-- WO UPDATE VIEW MODEL FIELDS
	[dbo].[Buildings].[Name] AS [BuildingName],
	[dbo].[WorkOrders].[CreatedDate],
	[dbo].[WorkOrders].[IsExpired],
	[dbo].[WorkOrders].[ClosingNotes],

	-- WO TASKS (CHILD LIsST)
	ISNULL([dbo].[WorkOrderTasks].[ID], -1) AS [Id],
	[dbo].[WorkOrderTasks].[WorkOrderId],
	[dbo].[WorkOrderTasks].[IsComplete],
	[dbo].[WorkOrderTasks].[Description],
	[dbo].[WorkOrderTasks].[CreatedDate],
	[dbo].[WorkOrderTasks].[UpdatedDate],
	[dbo].[WorkOrderTasks].[ServiceId],
	[dbo].[WorkOrderTasks].[UnitPrice],
	[dbo].[WorkOrderTasks].[Quantity],
	[dbo].[WorkOrderTasks].[DiscountPercentage],
	[dbo].[Services].[UnitFactor],
	[dbo].[Services].[Name],

	-- WO NOTES (CHILD LIsST)
	ISNULL([dbo].[WorkOrderNotes].[ID], -1) AS [Id],
	[dbo].[WorkOrderNotes].[WorkOrderId],
	[dbo].[WorkOrderNotes].[Note],
	[dbo].[WorkOrderNotes].[EmployeeId],
	[dbo].[WorkOrderNotes].[CreatedDate],
	[dbo].[WorkOrderNotes].[UpdatedDate],
	[dbo].[Employees].[Email] AS [EmployeeEmail],
	CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName],

	-- WO ATTACHMENTS (CHILD LIsST)
	ISNULL([dbo].[WorkOrderAttachments].[ID], -1) AS [Id],
	[dbo].[WorkOrderAttachments].[WorkOrderId],
	[dbo].[WorkOrderAttachments].[EmployeeId],
	[dbo].[WorkOrderAttachments].[BlobName],
	[dbo].[WorkOrderAttachments].[FullUrl],
	[dbo].[WorkOrderAttachments].[Description],
	[dbo].[WorkOrderAttachments].[ImageTakenDate]
FROM
	[dbo].[WorkOrders]
	LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[WorkOrders].[BuildingId] = [dbo].[Buildings].[ID]
	LEFT OUTER JOIN [dbo].[WorkOrderTasks] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderTasks].[WorkOrderId]
	LEFT OUTER JOIN [dbo].[Services] ON [dbo].[WorkOrderTasks].[ServiceId] = [dbo].[Services].[ID]
	LEFT OUTER JOIN [dbo].[WorkOrderNotes] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderNotes].[WorkOrderId]
	LEFT OUTER JOIN [dbo].[Employees] ON [dbo].[WorkOrderNotes].[EmployeeId] = [dbo].[Employees].[ID]
	LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
	LEFT OUTER JOIN [dbo].[WorkOrderAttachments] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderAttachments].[WorkOrderId]
WHERE 
	1 = 1 AND [dbo].[WorkOrders].[ID] = @workOrderId