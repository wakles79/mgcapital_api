using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Revenue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IRevenuesApplicationService : IBaseApplicationService<Revenue, int>
    {
        Task<DataSource<RevenueGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? status = -1, int? buildingId = null, int? customerId = null, int? contractId = null);

        Task<DataSource<RevenueGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request, int budgetId, int month, int year);

        Task<string> AddCSVExpense(RevenueCreateViewModel expenseVm);
    }
}
