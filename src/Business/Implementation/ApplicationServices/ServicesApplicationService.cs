using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Service;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ServicesApplicationService : BaseSessionApplicationService<Service, int>, IServicesApplicationService
    {
        public new IServicesRepository Repository => base.Repository as IServicesRepository;
        public ServicesApplicationService(
            IServicesRepository repository,
            IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }

        public Task<DataSource<ServiceListViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId, id);
        }

        public Task<DataSource<ServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId);
        }
    }
}
