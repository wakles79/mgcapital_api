// -----------------------------------------------------------------------
// <copyright file="IBuildingsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Building;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using System.Collections.Generic;
using System.Threading.Tasks;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.BuildingActivityLog;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IBuildingsApplicationService : IBaseApplicationService<Building, int>
    {
        /// <summary>
        ///     Reads all elements in a list-like form
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataSource<BuildingListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null, int? employeeId = null);

        Task<DataSource<BuildingGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = 1, int? isAvailable = -1, int? customerId = -1);

        Task<Building> FindNearestBuildingAsync(string address, double epsilon = 1e-2);

        Task<Building> FindNearestBuildingAsync(double latitude, double longitude, double epsilon = 1e-2);

        Task<IEnumerable<Building>> FindBuildingsInRadioAsync(double latitude, double longitude, double epsilon = 1e-2);

        Task<IEnumerable<Building>> FindBuildingsInRadioAsync(string address, double epsilon = 1e-2);

        Task<Address> SingleOrDefaultAddressAsync(int id);

        Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingId(int buildingId, BuildingEmployeeType type = BuildingEmployeeType.Any);

        Task<IEnumerable<int>> GetOpenWorkOrderIds(int buildingId);

        Task AssignEmployee(int buildingId, int employeeId, BuildingEmployeeType type);

        Task UnassignEmployee(int buildingId, int employeeId);

        Task<DataSource<BuildingListBoxViewModel>> ReadAllByContactCboDapperAsync(DataSourceRequest request, int? id = null, int? contactId = null);

        Task<IEnumerable<BuildingByOperationsManagerListBoxViewModel>> ReadAllBuildingsByOperationsManagerDapperAsync(DataSourceRequestBuildingsByOperationsManager request);

        Task UpdateEmployeeBuildings(BuildingUpdateEmployeeBuildingsViewModel vm);

        Task<DataSource<BuildingListBoxViewModel>> ReadAllByCustomerCboDapperAsync(DataSourceRequest request, int customerId, int? id);

        Task<string> GetBuildingReportBase64(int id);

        Task<string> GetBuildingsReportUrl(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1);

        Task UnassignEmployeesByBuildingIdAsync(int buildingId);

        Task AssignEmployeesDapperAsync(IEnumerable<EntityEmployee> buildingEmployees);

        Task<IEnumerable<BuildingWithLocationViewModel>> GetBuildingsWithLocationCboAsync();

        Task<IEnumerable<BuildingCsvViewModel>> ReadAllToCsv(DataSourceRequest request, int? isActive = 1, int? isAvailable = -1, int? customerId = -1);

        Task<IEnumerable<BuildingOperationManagerGridViewModel>> ReadAllSharedBuildingsFromOperationsManagerDapperAsync(DataSourceRequest request, int currentOperationsManager, int? operationsManager);

        Task UpdateSharedBuildingOperationManager(BuildingUpdateSharedBuildingsEmployeeViewModel vm);

        Task<DataSource<BuildingActivityLogGridViewModel>> ReadAllActivityLogAsync(DataSourceRequest request, int buildingId, int activityType = -1);
    }
}
