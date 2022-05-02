using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Directory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IDirectoryRepository : IBaseRepository<AuditableCompanyEntity, int>
    {
        Task<IEnumerable<DirectoryReadViewModel>> ReadAllDapperAsync(int companyId, string userEmail);

        Task<DirectoryDetailsEmployeeViewModel> EmployeeDetails(int employeeId);

        Task<DirectoryDetailsBuildingViewModel> BuildingDetails(int buildingId, int companyId, string userEmail);

        Task<DirectoryDetailsCustomerViewModel> CustomerDetails(int customerId);

        Task<DirectoryDetailsContactViewModel> ContactDetails(int contactId);
    }
}
