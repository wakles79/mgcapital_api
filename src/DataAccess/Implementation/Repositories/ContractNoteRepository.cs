// -----------------------------------------------------------------------
// <copyright file="ContractNoteRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.ContractNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ContractNoteRepository : BaseRepository<ContractNote, int>, IContractNoteRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public ContractNoteRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<ContractNoteGridViewModel>> ReadAllContractNotesAsync(int contractId, string currentEmailUser)
        {
            var result = new List<ContractNoteGridViewModel>();

            string query = $@"
                SELECT
	                CN.[ID],
	                CN.[ContractId],
	                CN.[EmployeeId],
	                CN.[CreatedDate],
	                CN.[UpdatedDate],
	                CN.[Note],
	                CN.[CreatedBy] AS EmployeeEmail,
	                CONCAT(T.[FirstName], ' ', T.[MiddleName], ' ', T.[LastName]) AS [EmployeeFullName],
                    CASE WHEN CN.[CreatedBy] = @UserEmail THEN 1 ELSE 0 END AS Me
                FROM [dbo].[ContractNotes] AS CN
	                LEFT JOIN [dbo].[Contracts] AS C ON C.[ID] = CN.[ContractId]
	                LEFT OUTER JOIN [dbo].[Employees] AS E ON E.[ID] = CN.[EmployeeId]
	                LEFT OUTER JOIN [dbo].[Contacts] AS T ON T.[ID] = E.[ContactId]
                WHERE CN.[ContractId] = @ContractId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ContractId", contractId);
            parameters.Add("@UserEmail", currentEmailUser);

            var rows = await this._baseDapperRepository.QueryAsync<ContractNoteGridViewModel>(query, parameters);

            if (rows.Any())
            {
                result = rows.ToList();
            }

            return result;
        }
    }
}
