using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CleaningReportItem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ICleaningReportItemsRepository : IBaseRepository<CleaningReportItem,int>
    {
        Task<IEnumerable<CleaningReportItemGridViewModel>> GetCleaningReportItemsDapper(int companyId, int cleaningReportId, int? type = null);

        Task<CleaningReportItemUpdateViewModel> GetCleaningReportItemDapperAsync(int id);
    }
}
