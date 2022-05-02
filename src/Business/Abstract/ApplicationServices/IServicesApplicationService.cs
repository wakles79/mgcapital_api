using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IServicesApplicationService : IBaseApplicationService<Service, int>
    {
        Task<DataSource<ServiceListViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null);

        Task<DataSource<ServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request);
    }
}
