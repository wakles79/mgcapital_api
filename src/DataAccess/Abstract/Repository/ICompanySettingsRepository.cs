// -----------------------------------------------------------------------
// <copyright file="ICompanySettingsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CompanySettings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="CompanySettings"/>
    /// </summary>
    public interface ICompanySettingsRepository : IBaseRepository<CompanySettings, int>
    {
        Task<CompanySettingsDetailViewModel> GetCompanySettingsDapperAsync(int companyId);

        void UpdateCompanySettingDapperAsync(CompanySettings settings);
    }
}
