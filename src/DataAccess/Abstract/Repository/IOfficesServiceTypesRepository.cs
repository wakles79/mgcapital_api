// -----------------------------------------------------------------------
// <copyright file="IOfficeServiceTypesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.OfficeServiceType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="OfficeServiceType"/>
    /// </summary>
    public interface IOfficeServiceTypesRepository : IBaseRepository<OfficeServiceType, int>
    {
        Task<DataSource<OfficeServiceTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isEnabled = -1);

        Task<DataSource<OfficeServiceTypeListViewModel>> ReadAllCboDapperAsync(int companyId, int status = -1, int rateType = -1, string exclude = "");

        Task<IEnumerable<OfficeServiceType>> FindByName(int companyId, string name);
    }
}