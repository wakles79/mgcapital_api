DECLARE @CompanyId AS INT = 1
DECLARE @UserEmail AS NVARCHAR(128) = 'mgclient.one@testing.com'

SELECT 
	[dbo].[CleaningReports].[Id],
	[dbo].[CleaningReports].[Guid],
	[dbo].[CleaningReports].[DateOfService] AS [Date],
	[dbo].[CleaningReports].[Location],
	[CleaningItemsCount] = (SELECT COUNT(Id) FROM [dbo].[CleaningReportItems] 
							WHERE [dbo].[CleaningReportItems].CleaningReportId = [dbo].[CleaningReports].[Id] AND [dbo].[CleaningReportItems].[Type] = 0),
	[FindingItemsCount] = (SELECT COUNT(Id) FROM [dbo].[CleaningReportItems] 
							WHERE [dbo].[CleaningReportItems].CleaningReportId = [dbo].[CleaningReports].[Id] AND [dbo].[CleaningReportItems].[Type] = 1)

FROM
	[dbo].[CleaningReports]
	LEFT OUTER JOIN [dbo].[BuildingContacts] ON [dbo].[CleaningReports].[ContactId] = [dbo].[BuildingContacts].[ContactId]
	LEFT OUTER JOIN [dbo].[Contracts] ON [dbo].[BuildingContacts].[BuildingId] = [dbo].[Contracts].[BuildingId]
	LEFT OUTER JOIN [dbo].[CustomerUsers] ON [dbo].[Contracts].[CustomerId] = [dbo].[CustomerUsers].[CustomerId]

WHERE
	[dbo].[CustomerUsers].[CompanyId] = @CompanyId 
	AND [dbo].[CustomerUsers].[Email] = @UserEmail
	AND [dbo].[CleaningReports].[Submitted] <> 0

ORDER BY
	[dbo].[CleaningReports].[DateOfService]
