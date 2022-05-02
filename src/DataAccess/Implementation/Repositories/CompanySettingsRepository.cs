// -----------------------------------------------------------------------
// <copyright file="CompanySettingsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CompanySettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="CompanySettings"/>
    /// </summary>
    public class CompanySettingsRepository : BaseRepository<CompanySettings, int>, ICompanySettingsRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public CompanySettingsRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<CompanySettingsDetailViewModel> GetCompanySettingsDapperAsync(int companyId)
        {
            CompanySettingsDetailViewModel companySettings = new CompanySettingsDetailViewModel();

            string query = $@"
                SELECT
	                ISNULL(CS.[ID], 0) AS ID,
                    C.[ID] AS CompanyId,
	                C.[Name] AS CompanyName,
	                ISNULL(CS.[MinimumProfitMarginPercentage], 0) AS MinimumProfitMarginPercentage,
                    ISNULL(CS.[FederalInsuranceContributionsAct], 0) AS FederalInsuranceContributionsAct,
                    ISNULL(CS.[Medicare], 0) AS Medicare,
                    ISNULL(CS.[FederalUnemploymentTaxAct], 0) AS FederalUnemploymentTaxAct,
                    ISNULL(CS.[StateUnemploymentInsurance], 0) AS StateUnemploymentInsurance,
                    ISNULL(CS.[WorkersCompensation], 0) AS WorkersCompensation,
                    ISNULL(CS.[GeneralLedger], 0) AS GeneralLedger,
                    ISNULL(CS.[StateTax], 0) AS stateTax,
                    ISNULL(CS.[FreshdeskEmail], '') AS [FreshdeskEmail],
                    ISNULL(CS.[FreshdeskDefaultAgentId], '') AS [FreshdeskDefaultAgentId],
                    ISNULL(CS.[FreshdeskDefaultApiKey], '') AS [FreshdeskDefaultApiKey],
                    ISNULL(CS.[LogoBlobName], '') AS [LogoBlobName],
                    ISNULL(CS.[LogoFullUrl], '') AS [LogoFullUrl],
                    ISNULL(CS.[GmailEnabled], 0) AS [GmailEnabled],
                    ISNULL(CS.[GmailEmail], '') AS [GmailEmail],
                    ISNULL(CS.[LastHistoryId], 0) AS [LastHistoryId],
                    ISNULL(CS.[EmailSignature], '') AS [EmailSignature]
                FROM [dbo].[Companies] AS C
	                LEFT JOIN CompanySettings AS CS ON CS.CompanyId = C.ID
                WHERE C.ID = @CompanyId
            ";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);

            var result = await _baseDapperRepository.QueryAsync<CompanySettingsDetailViewModel>(query, pars);

            if (result.Any())
                companySettings = result.First();

            return companySettings;
        }

        public void UpdateCompanySettingDapperAsync(CompanySettings settings)
        {
            string gmailEnabled = "0";
            if (settings.GmailEnabled)
                gmailEnabled = "1";
            string query = $@"UPDATE [dbo].[CompanySettings]
                SET GmailEnabled = {gmailEnabled},
                    LastHistoryId = {settings.LastHistoryId},
                    GmailEmail = '{settings.GmailEmail}'
                WHERE ID = @CompanyId
                ";
            var pars = new DynamicParameters();
            pars.Add("@CompanyId", settings.CompanyId);
            this._baseDapperRepository.Execute(query, pars);
        }

    }
}
