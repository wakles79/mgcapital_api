declare @workOrderId int;

select @workOrderId = 5
-- payload query

SELECT 
		[ID]
      ,[CreatedDate]
      ,[Description]
      ,[IsComplete]
      ,[UpdatedDate]
      ,[WorkOrderId]
  FROM [dbo].[WorkOrderTasks]
  WHERE [WorkOrderId] = @workOrderId