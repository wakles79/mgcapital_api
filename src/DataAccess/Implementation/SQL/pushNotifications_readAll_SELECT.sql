DECLARE @useremail AS NVARCHAR(128) = 'daileny@axzes.com'
DECLARE @companyId AS INT = 1
                
DECLARE @employeeId AS INT

SELECT @employeeId = ISNULL((SELECT [Id] FROM [dbo].[Employees] WHERE [Email] = @UserEmail AND [CompanyId] = @companyId), -1) ;

SELECT * FROM ( 
    SELECT ROW_NUMBER() OVER(ORDER BY IIF(ISNULL([dbo].[PushNotificationConverts].[EmployeeId], -1) = -1, 1, 0) DESC, [dbo].[PushNotifications].[CompletedAt] ) AS PagedNumber, COUNT(*) OVER() AS [Count], 
[dbo].[PushNotifications].[Id], 
[dbo].[PushNotifications].[Heading],
[dbo].[PushNotifications].[Content],
[dbo].[PushNotifications].[CompletedAt] AS [Completed_At],
[dbo].[PushNotifications].[Reason],
[dbo].[PushNotifications].[OneSignalId],
[dbo].[PushNotifications].[DataType],
[dbo].[PushNotifications].[Data],
IIF(ISNULL([dbo].[PushNotificationConverts].[EmployeeId], -1) = -1, 1, 0) AS [Unread]  
    FROM 
[dbo].[PushNotifications]
INNER JOIN [dbo].[PushNotificationFilters] ON [dbo].[PushNotificationFilters].[PushNotificationId] = [dbo].[PushNotifications].[Id]
LEFT OUTER JOIN [dbo].[PushNotificationConverts] ON [dbo].[PushNotificationConverts].[PushNotificationId] = [dbo].[PushNotifications].[ID] 
													AND [dbo].[PushNotificationConverts].[EmployeeId] = @employeeId
    WHERE 1 = 1 
AND [dbo].[PushNotificationFilters].[Field] = 'tag' 
AND [dbo].[PushNotificationFilters].[Key] = 'user_email' 
AND [dbo].[PushNotificationFilters].[Relation] = '=' 
AND [dbo].[PushNotificationFilters].[Value] = @UserEmail 
	-- EXTRA FILTERS
    --AND [dbo].[PushNotifications].[Reason] = 1  
    --AND IIF([dbo].[PushNotificationConverts].[EmployeeId] = @employeeId, 0, 1) = 1 
    --AND [dbo].[PushNotifications].[CompletedAt]  >  1539057600
    --AND [dbo].[PushNotifications].[CompletedAt]  <  1539230399 
) AS q
--WHERE PagedNumber BETWEEN ( 0 * 20 + 1 ) AND ( (0 + 1) * 20 ) 
