using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Revenue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IRevenuesRepository : IBaseRepository<Revenue, int>
    {
        Task<DataSource<RevenueGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? status, int? buildingId = null, int? customerId = null, int? contractId = null);

        Task<DataSource<RevenueGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request,int companyId, int budgetId, int month, int year);

        Task<DataSource<RevenueGridViewModel>> GetRepeated(int contractId, string reference, double total, int companyId);
    }
}
