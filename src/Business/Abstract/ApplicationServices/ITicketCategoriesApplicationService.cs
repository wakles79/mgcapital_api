using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.TicketCategory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ITicketCategoriesApplicationService : IBaseApplicationService<TicketCategory, int>
    {
        Task<DataSource<TicketCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request);
    }
}
