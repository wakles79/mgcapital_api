using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Inspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionNotesRepository : BaseRepository<InspectionNote, int>, IInspectionNotesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public InspectionNotesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
        : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<InspectionNoteGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int inspectionID)
        {
            var result = new DataSource<InspectionNoteGridViewModel>
            {
                Payload = new List<InspectionNoteGridViewModel>(),
                Count = 0
            };

            string query = $@"
                 SELECT TOP (1000) II.[ID]
                      , II.[CreatedDate]
                      ,II.[CreatedBy]
	                  ,CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName]
                      ,II.[UpdatedDate]
                      ,II.[UpdatedBy]
                      ,II.[InspectionId]
                      ,II.[EmployeeId]
                      ,II.[Note]
                  FROM [dbo].[InspectionNotes] AS II
  	                    LEFT OUTER JOIN [dbo].[Employees] ON II.[EmployeeId] = [dbo].[Employees].[ID]
	                    LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
                  WHERE II.[InspectionId] = @inspectionId";

            var pars = new DynamicParameters();
            pars.Add("@inspectionId", inspectionID);

            var payload = await _baseDapperRepository.QueryAsync<InspectionNoteGridViewModel>(query, pars);

            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
