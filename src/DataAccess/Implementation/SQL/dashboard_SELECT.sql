
DECLARE @roleLevel INT;
DECLARE @employeeId INT = 13;
DECLARE @companyId INT = 5;

SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = @employeeId

select * from (
	select 
		(SELECT  COUNT(*) as 'Quantity-DueTodayTotal'
		 FROM WorkOrders as W
		 LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		 WHERE CAST(W.DueDate AS DATE) = CAST(GETDATE() AS DATE) AND
		 W.CompanyId = @companyId AND
	     AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'quantity', 

		(SELECT COUNT(*) as 'Footer-CompletedTodayTotal'
		 FROM WorkOrders as W
		 LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		 WHERE CAST(W.DueDate AS DATE) = CAST(GETDATE() AS DATE) AND
		 W.StatusId = 3 AND
		 W.CompanyId = @companyId AND
		 AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		0 as 'criteria'
		
UNION ALL

	select  
		(SELECT COUNT(*) as 'Quantity-OverdueTotal'
		 FROM WorkOrders as W
		 LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		 WHERE W.IsExpired = 1 AND 
	     W.StatusId IN (1, 2) AND 
		 W.CompanyId = @companyId AND
		 AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'quantity', 

		(SELECT COUNT(*) as 'Footer-YesterdayOverdueTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		WHERE W.IsExpired = 1 AND 
	    W.StatusId IN (1, 2) AND
		CAST(W.DueDate AS DATE) = CAST(DATEADD(d, -1, GETDATE()) AS DATE) AND
		W.CompanyId = @companyId AND
		W.AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN W.AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		1 as 'criteria'

UNION ALL

	select 
		(SELECT COUNT(*) as 'Quantity-StandByTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		WHERE W.StatusId = 1 AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END)  as 'quantity', 

		(SELECT COUNT(*) as 'Footer-AssignedThisWeekTotal' --*
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		LEFT OUTER JOIN [dbo].WorkOrderStatusLog as L on L.WorkOrderId = W.ID
		WHERE L.StatusId = 1 AND
		CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETDATE()), CAST(GETDATE() AS DATE)) AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		2 as 'criteria'

UNION ALL

	select 
		(SELECT COUNT(*) as 'Quantity-OpenTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		WHERE W.StatusId != 3 AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'quantity', 
		
		(SELECT COUNT(*) as 'Footer-ClosedTodayTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		LEFT OUTER JOIN [dbo].WorkOrderStatusLog as L on L.WorkOrderId = W.ID
		WHERE W.StatusId = 3 AND
		CAST(L.CreatedDate AS DATE) = CAST(GETDATE() AS DATE) AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		3 as 'criteria'

UNION ALL

	select
		(SELECT COUNT(*) as 'Quantity-DraftTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		WHERE W.StatusId = 0 AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'quantity', 

		(SELECT COUNT(*) as 'Footer-DraftThisWeekTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		LEFT OUTER JOIN [dbo].WorkOrderStatusLog as L on L.WorkOrderId = W.ID
		WHERE L.StatusId = 0 AND
		CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETDATE()), CAST(GETDATE() AS DATE)) AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		4 as 'criteria'

UNION ALL

	select 
		0 as 'quantity', 
		(SELECT COUNT(*) as 'FooterValue-CreatedThisWeekTotal'
		FROM WorkOrders as W
		LEFT OUTER JOIN [dbo].[Employees] as E on E.[ID] = W.AssignedEmployeeId
		LEFT OUTER JOIN [dbo].WorkOrderStatusLog as L on L.WorkOrderId = W.ID
		WHERE CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETDATE()), CAST(GETDATE() AS DATE)) AND
		W.CompanyId = @companyId AND
		AssignedEmployeeId = CASE WHEN @roleLevel <= 20 THEN AssignedEmployeeId ELSE  @employeeId END) as 'FooterValue', 
		5 as 'criteria'

) as result