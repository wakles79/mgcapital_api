// -----------------------------------------------------------------------
// <copyright file="IOfficeServiceTypesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.OfficeServiceType;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IOfficeServiceTypesApplicationService : IBaseApplicationService<OfficeServiceType, int>
    {

        Task<DataSource<OfficeServiceTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isEnabled = -1);

        Task<DataSource<OfficeServiceTypeListViewModel>> ReadAllCboDapperAsync(int status = -1, int rateType = -1, string exclude = "");
    }
}
