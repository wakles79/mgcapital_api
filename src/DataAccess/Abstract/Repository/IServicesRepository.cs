using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IServicesRepository : IBaseRepository<Service, int>
    {
        Task<DataSource<ServiceListViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null);

        Task<DataSource<ServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId);
    }
}
