using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Directory;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class DirectoryApplicationService : BaseSessionApplicationService<AuditableCompanyEntity, int>, IDirectoryApplicationService
    {
        private readonly IDirectoryRepository _directoryRepository;

        public DirectoryApplicationService(IDirectoryRepository repository, 
            IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _directoryRepository = repository;
        }

        public async Task<IEnumerable<DirectoryReadViewModel>> ReadAllAsync()
        {
            var result = await _directoryRepository.ReadAllDapperAsync(this.CompanyId, this.UserEmail);
            return result;
        }

        public async Task<DirectoryDetailsEmployeeViewModel> EmployeeDetails(int employeeId)
        {
            var result = await _directoryRepository.EmployeeDetails(employeeId);
            return result;
        }

        public async Task<DirectoryDetailsBuildingViewModel> BuildingDetails(int buildingId)
        {
            var result = await _directoryRepository.BuildingDetails(buildingId, CompanyId, UserEmail);
            return result;
        }

        public async Task<DirectoryDetailsCustomerViewModel> CustomerDetails(int customerId)
        {
            var result = await _directoryRepository.CustomerDetails(customerId);
            return result;
        }

        public async Task<DirectoryDetailsContactViewModel> ContactDetails(int contactId)
        {
            var result = await _directoryRepository.ContactDetails(contactId);
            return result;
        }
    }
}
