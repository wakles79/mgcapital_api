using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IDirectoryApplicationService : IBaseApplicationService<AuditableCompanyEntity, int>
    {
        Task<IEnumerable<DirectoryReadViewModel>> ReadAllAsync();

        Task<DirectoryDetailsEmployeeViewModel> EmployeeDetails(int employeeId);

        Task<DirectoryDetailsBuildingViewModel> BuildingDetails(int buildingId);

        Task<DirectoryDetailsCustomerViewModel> CustomerDetails(int customerId);

        Task<DirectoryDetailsContactViewModel> ContactDetails(int contactId);
    }
}
