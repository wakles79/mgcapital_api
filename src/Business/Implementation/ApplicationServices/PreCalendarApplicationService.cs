using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PreCalendar;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class PreCalendarApplicationService : BaseSessionApplicationService<PreCalendar, int>, IPreCalendarApplicationService
    {

        public new IPreCalendarRepository Repository => base.Repository as IPreCalendarRepository;
        public IPreCalendarTasksRepository PCTasksRepository { get; private set; }

        public PreCalendarApplicationService(
            IPreCalendarRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IEmployeesRepository employeesRepository,
            IPreCalendarTasksRepository pcTasksRepository
        ) : base(repository, httpContextAccessor)
        {
            this.PCTasksRepository = pcTasksRepository;
        }

        public Task<DataSource<PreCalendarGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? buildingId = null)
        {
            return Repository.ReadAllDapperAsync(request, this.CompanyId, buildingId);
        }

        public Task RemoveTasksAsync(int objId)
        {
            return this.PCTasksRepository.RemoveAsync(objId);
        }

        public Task<PreCalendarDetailViewModel> SingleOrDefaultDapperAsync(int id)
        {
            return this.Repository.SingleOrDefaultDapperAsync(id);
        }
    }
}
