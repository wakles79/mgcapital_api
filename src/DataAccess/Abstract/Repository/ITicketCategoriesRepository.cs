using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.TicketCategory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ITicketCategoriesRepository : IBaseRepository<TicketCategory, int>
    {
        Task<DataSource<TicketCategoryGridViewModel>> ReadAllDapperAsync(int companyId, DataSourceRequest request);
    }
}
