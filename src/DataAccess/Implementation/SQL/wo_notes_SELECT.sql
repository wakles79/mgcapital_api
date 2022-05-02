declare @workOrderId int;

select @workOrderId = 5
-- payload query

SELECT 
		wo.[ID]
      ,wo.[CreatedDate]
      ,wo.[UpdatedDate]
      ,wo.[WorkOrderId]
	  ,wo.[Note]
	  ,emp.[Email] as EmployeeEmail
	  ,emp.[ID] as EmployeeId
FROM [dbo].[WorkOrderNotes] as wo
	INNER JOIN [dbo].[Employees] as emp ON wo.EmployeeId = emp.ID
WHERE [WorkOrderId] = @workOrderId