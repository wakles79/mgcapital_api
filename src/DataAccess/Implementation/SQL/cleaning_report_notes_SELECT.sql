DECLARE @cleaningReportId AS INT = 16

SELECT 
    [dbo].[CleaningReportNotes].[Id],
    [dbo].[CleaningReportNotes].[CleaningReportId],
    [dbo].[CleaningReportNotes].[CreatedDate],
    [dbo].[CleaningReportNotes].[Direction],
    CASE WHEN [dbo].[CleaningReportNotes].[SenderId] IS NULL 
		THEN 'Customer' 
		ELSE (
			SELECT TOP(1) CONCAT_WS(' ', c.FirstName, c.LastName)
			FROM [dbo].[Contacts] AS c
				LEFT OUTER JOIN [dbo].[Employees] AS e ON e.ContactId = c.ID
				LEFT OUTER JOIN [dbo].[CleaningReportNotes] AS crn ON crn.SenderId = e.ID)
	END AS [SenderName],
    [dbo].[CleaningReportNotes].[Message]
FROM
    [dbo].[CleaningReportNotes]
	LEFT OUTER JOIN [dbo].[CleaningReports] ON [dbo].[CleaningReportNotes].[CleaningReportId] = [dbo].[CleaningReports].[Id] 
WHERE 
	[dbo].[CleaningReports].[Guid] = 'ad8e2a63-8ac4-4b07-a77d-91acfcdadd84'
--	[dbo].[CleaningReportNotes].[CleaningReportId] = @cleaningReportId

--update CleaningReportNotes set cleaningReportId = 16