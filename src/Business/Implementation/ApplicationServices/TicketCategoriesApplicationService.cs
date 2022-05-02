using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.TicketCategory;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class TicketCategoriesApplicationService : BaseSessionApplicationService<TicketCategory, int>, ITicketCategoriesApplicationService
    {
        protected new ITicketCategoriesRepository Repository => base.Repository as ITicketCategoriesRepository;

        public TicketCategoriesApplicationService(
            ITicketCategoriesRepository repository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor)
        {

        }
        public Task<DataSource<TicketCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllDapperAsync(this.CompanyId, request);
        }
    }
}
