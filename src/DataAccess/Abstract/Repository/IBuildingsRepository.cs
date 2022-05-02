// -----------------------------------------------------------------------
// <copyright file="IBuildingsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Building;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface IBuildingsRepository : IBaseRepository<Building, int>
    {
        Task<DataSource<BuildingListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null, int? employeeId = null);

        Task<DataSource<BuildingGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1, int? isAvailable = -1, int? customerId = -1);

        Task<Building> FindNearestBuildingAsync(double latitude, double longitude, double epsilon, int companyId);

        Task<IEnumerable<Building>> FindBuildingsInRadioAsync(double latitude, double longitude, double epsilon, int companyId);
        
        Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingId_DapperAsync(int buildingId, BuildingEmployeeType type);

        Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingIdsDapperAsync(IEnumerable<int> buildingIds, BuildingEmployeeType type);

        Task<IEnumerable<int>> GetOpenWorkOrderIds(int buildingId);

        Task<DataSource<BuildingListBoxViewModel>> ReadAllByContactCboDapperAsync(DataSourceRequest request, int companyId, int? id = null, int? contactId = null);

        Task<IEnumerable<BuildingByOperationsManagerListBoxViewModel>> ReadAllBuildingsByOperationsManagerDapperAsync(DataSourceRequestBuildingsByOperationsManager request, int companyId);
        Task<DataSource<BuildingListBoxViewModel>> ReadAllByCustomerCboDapperAsync(DataSourceRequest request, int companyId, int customerId, int? id);

        Task<DataSource<BuildingReportGridViewModel>> ReadAllWithCustomerDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1, int? isAvailable = -1);
        Task UnassignEmployeesByBuildingIdAsync(int buildingId);
        Task AssignEmployeesDapperAsync(IEnumerable<EntityEmployee> buildingEmployees);
        Task<IEnumerable<BuildingWithLocationViewModel>> GetBuildingsWithLocationCboAsync(int companyId);

        Task<Building> GetBuildingByContractNumber(string buldingCode);

        Task<IEnumerable<BuildingOperationManagerGridViewModel>> GetSharedBuildingsFromOperationsManagerDapperAsync(DataSourceRequest request, int companyId, int currentOperationsManager, int? operationsManager);
    }
}
