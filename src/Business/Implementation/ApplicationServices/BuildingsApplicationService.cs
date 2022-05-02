using Geocoding;
using Geocoding.Google;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Building;
using MGCap.Domain.ViewModels.BuildingActivityLog;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contact;
using MGCap.Domain.ViewModels.Employee;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
	public class BuildingsApplicationService : BaseSessionApplicationService<Building, int>, IBuildingsApplicationService
	{

		public new IBuildingsRepository Repository => base.Repository as IBuildingsRepository;

		private readonly IBuildingEmployeesRepository _buildingEmployeesRepository;

		public IEmployeesRepository EmployeesRepository { get; private set; }

		private IPDFGeneratorApplicationService PDFGeneratorApplicationService { get; set; }

		private readonly IBuildingContactsRepository BuildingContactsRepository;

		private readonly IContractsRepository ContractRepository;

		private readonly ICustomersRepository CustomersRepository;

		private readonly IBuildingActivityLogRepository ActivityLogRepository;

		/// <summary>
		///     Gets a google geocoder object
		/// </summary>
		public readonly IGeocoder Geocoder;
		public IAddressesRepository AddressRepository { get; set; }
		public BuildingsApplicationService(
			IBuildingsRepository repository,
			IAddressesRepository addressRepository,
			IHttpContextAccessor httpContextAccessor,
			IBuildingEmployeesRepository buildingEmployeesRepository,
			IEmployeesRepository employeesRepository,
			IPDFGeneratorApplicationService pDFGeneratorApplicationService,
			IBuildingContactsRepository buildingContactsRepository,
			IContractsRepository contractRepository,
			ICustomersRepository customersRepository,
			IBuildingActivityLogRepository activityLogRepository,
		IOptions<GeocoderOptions> geocoderOptions) : base(repository, httpContextAccessor)
		{
			this.AddressRepository = addressRepository;
			_buildingEmployeesRepository = buildingEmployeesRepository;

			this.EmployeesRepository = employeesRepository;

			this.PDFGeneratorApplicationService = pDFGeneratorApplicationService;
			this.BuildingContactsRepository = buildingContactsRepository;
			this.ContractRepository = contractRepository;
			this.CustomersRepository = customersRepository;
			this.ActivityLogRepository = activityLogRepository;

			this.Geocoder = new GoogleGeocoder
			{
				ApiKey = geocoderOptions.Value.GoogleGeocoderApiKey
			};
		}

		public Task<DataSource<BuildingListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null, int? employeeId = null)
		{
			return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId, id, employeeId);
		}

		public Task<DataSource<BuildingGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = 1, int? isAvailable = -1, int? customerId = -1)
		{
			return this.Repository.ReadAllDapperAsync(request, this.CompanyId, isActive, isAvailable, customerId);
		}

		public Task<Domain.Models.Address> SingleOrDefaultAddressAsync(int id)
		{
			return this.AddressRepository.SingleOrDefaultAsync(id);
		}

		public async Task<Building> FindNearestBuildingAsync(string address, double epsilon = 1e-2)
		{
			var coordinates = await this.GetCoordinatesAsync(address);
			if (coordinates == null)
			{
				return null;
			}
			return await this.FindNearestBuildingAsync(coordinates.Latitude, coordinates.Longitude, epsilon);
		}

		public Task<Building> FindNearestBuildingAsync(double latitude, double longitude, double epsilon = 1e-2)
		{
			return this.Repository.FindNearestBuildingAsync(latitude, longitude, epsilon, this.CompanyId);
		}

		public Task<IEnumerable<Building>> FindBuildingsInRadioAsync(double latitude, double longitude, double epsilon = 1e-2)
		{
			return this.Repository.FindBuildingsInRadioAsync(latitude, longitude, epsilon, this.CompanyId);
		}

		public async Task<IEnumerable<Building>> FindBuildingsInRadioAsync(string address, double epsilon = 1e-2)
		{
			var coordinates = await this.GetCoordinatesAsync(address);
			if (coordinates == null)
			{
				return new List<Building>();
			}
			return await this.FindBuildingsInRadioAsync(coordinates.Latitude, coordinates.Longitude, epsilon);
		}

		private async Task<Location> GetCoordinatesAsync(string address)
		{
			try
			{
				var addresses = await this.Geocoder.GeocodeAsync(address);
				if (addresses.Count() == 1)
				{
					var gAddress = addresses.First();
					return gAddress.Coordinates;
				}
			}
			catch (GoogleGeocodingException gEx)
			{
#if DEBUG
				Console.WriteLine(gEx.Message);
#endif
			}
			catch (ArgumentException aEx)
			{
#if DEBUG
				Console.WriteLine(aEx.Message);
#endif
			}
			catch (Exception ex)
			{
#if DEBUG
				Console.WriteLine(ex.Message);
#endif
			}
			return null;
		}

		public async Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingId(int buildingId, BuildingEmployeeType type = BuildingEmployeeType.Any)
		{
			var result = await Repository.GetEmployeesByBuildingId_DapperAsync(buildingId, type);
			return result;
		}

		public async Task<IEnumerable<int>> GetOpenWorkOrderIds(int buildingId)
		{
			var result = await Repository.GetOpenWorkOrderIds(buildingId);
			return result;
		}

		public async Task AssignEmployee(int buildingId, int employeeId, BuildingEmployeeType type)
		{
			var obj = new BuildingEmployee
			{
				BuildingId = buildingId,
				EmployeeId = employeeId,
				Type = type
			};

			await _buildingEmployeesRepository.AddAsync(obj);
		}

		public async Task UnassignEmployee(int buildingId, int employeeId)
		{
			await _buildingEmployeesRepository.RemoveAsync(b => b.BuildingId.Equals(buildingId) && b.EmployeeId.Equals(employeeId));
		}

		public async Task<DataSource<BuildingListBoxViewModel>> ReadAllByContactCboDapperAsync(DataSourceRequest request, int? id = null, int? contactId = null)
		{
			return await Repository.ReadAllByContactCboDapperAsync(request, CompanyId, id, contactId);
		}

		public async Task<IEnumerable<BuildingByOperationsManagerListBoxViewModel>> ReadAllBuildingsByOperationsManagerDapperAsync(DataSourceRequestBuildingsByOperationsManager request)
		{
			if (!request.CurrentEmployeeId.HasValue)
			{
				request.CurrentEmployeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
			}

			return await Repository.ReadAllBuildingsByOperationsManagerDapperAsync(request, CompanyId);
		}

		public async Task UpdateEmployeeBuildings(BuildingUpdateEmployeeBuildingsViewModel vm)
		{
			try
			{
				IEnumerable<int> buildingsByEmployee = this.GetBuildingsByEmployeeIdAndType(vm.EmployeeId, vm.Type);

				if (buildingsByEmployee.Any())
				{
					foreach (int buildingId in buildingsByEmployee)
					{
						await UnassignEmployee(buildingId, vm.EmployeeId);
					}
				}

				if (vm.BuildingsId.Any())
				{
					foreach (int buildingId in vm.BuildingsId)
					{
						await AssignEmployee(buildingId, vm.EmployeeId, vm.Type);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private IEnumerable<int> GetBuildingsByEmployeeIdAndType(int employeeId, BuildingEmployeeType type)
		{
			return _buildingEmployeesRepository.GetBuildingsByEmployeeIdAndType(employeeId, type);
		}

		public Task<DataSource<BuildingListBoxViewModel>> ReadAllByCustomerCboDapperAsync(DataSourceRequest request, int customerId, int? id)
		{
			return Repository.ReadAllByCustomerCboDapperAsync(request, CompanyId, customerId, id);
		}

		public async Task<string> GetBuildingReportBase64(int id)
		{
			try
			{
				var building = this.Repository.SingleOrDefault(b => b.ID == id);

				var contract = await this.ContractRepository.SingleOrDefaultContractByBuildingAsync(id);
				string customerName = string.Empty;
				if (contract != null)
				{
					var customer = this.CustomersRepository.SingleOrDefault(c => c.ID == contract.CustomerId);
					customerName = customer.Name;
				}

				JObject joBuilding = new JObject();
				joBuilding.Add("BuildingName", building.Name);
				joBuilding.Add("AddressFull", building.Address.FullAddress);
				joBuilding.Add("MgmtCompany", customerName);
				joBuilding.Add("EmergencyPhone", building.EmergencyPhone);
				joBuilding.Add("Extension", building.EmergencyPhoneExt);
				joBuilding.Add("EmergencyNote", building.EmergencyNotes);

				var contacts = await this.BuildingContactsRepository.ReadAllAsyncDapper(new DataSourceRequest(), id);

				var jsonContacts = contacts.Payload == null ? "[]" : JsonConvert.SerializeObject(contacts.Payload);

				joBuilding.Add("Contacts", JArray.Parse(jsonContacts));

				var result = await this.PDFGeneratorApplicationService.GetBase64Document("54797", joBuilding.ToString());
				return result;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<string> GetBuildingsReportUrl(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1)
		{
			try
			{
				var buildings = await this.Repository.ReadAllWithCustomerDapperAsync(request, this.CompanyId, isActive, isAvailable);

				IEnumerable<int> buildingIds = buildings.Payload?.Select(e => e.ID);

				var contacts = await this.BuildingContactsRepository.ReadAllByBuildingIdsAsyncDapper(new DataSourceRequest { PageSize = request.PageSize }, buildingIds);

				var employees = await this.Repository.GetEmployeesByBuildingIdsDapperAsync(
					buildingIds,
					BuildingEmployeeType.OperationsManager | BuildingEmployeeType.Supervisor);

				foreach (var building in buildings.Payload)
				{
					// Only Property Managers
					building.Contacts = contacts.Payload
												.Where(
													c => c.BuildingId == building.ID &&
													new WorkOrderContactType(c.Type).Key == WorkOrderContactType.PropertyManager.Key
													);

					var buildingEmployees = employees.Where(e => e.BuildingId == building.ID);

					if (buildingEmployees?.Any() == true)
					{
						building.Contacts = building.Contacts.Union(
							buildingEmployees.Select(e => new ContactGridViewModel
							{
								FullName = e.Name,
								Type = e.Type.ToString().SplitCamelCase(),
								Email = e.Email,
								Phone = e.Phone,
							}));
					}
				}

				string jsonBuildings = JsonConvert.SerializeObject(buildings.Payload);
				JArray jaBuildings = JArray.Parse(jsonBuildings);

				JObject joBody = new JObject
				{
					{ "Payload", jaBuildings }
				};

				string url = await this.PDFGeneratorApplicationService.GetDocumentUrl("55044", joBody.ToString());

				return url;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public Task UnassignEmployeesByBuildingIdAsync(int buildingId)
		{
			return this.Repository.UnassignEmployeesByBuildingIdAsync(buildingId);
		}

		public Task AssignEmployeesDapperAsync(IEnumerable<EntityEmployee> buildingEmployees)
		{
			return this.Repository.AssignEmployeesDapperAsync(buildingEmployees);
		}

		public Task<IEnumerable<BuildingWithLocationViewModel>> GetBuildingsWithLocationCboAsync()
		{
			return this.Repository.GetBuildingsWithLocationCboAsync(this.CompanyId);
		}

		public async Task<IEnumerable<BuildingCsvViewModel>> ReadAllToCsv(DataSourceRequest request, int? isActive = 1, int? isAvailable = -1, int? customerId = -1)
		{
			var buildings = await this.Repository.ReadAllDapperAsync(request, this.CompanyId, isActive, isAvailable, customerId);

			IEnumerable<BuildingCsvViewModel> results = buildings.Payload
				.Select(b => new BuildingCsvViewModel()
				{
					Address = b.FullAddress,
					BuildingCode = b.Code,
					CustomerCode = b.CustomerCode,
					EmergencyPhone = b.EmergencyPhone,
					IsActive = b.IsActive == 1 ? "Active" : "Not Active",
					IsComplete = b.IsComplete == 1 ? "Yes" : "No",
					Name = b.Name,
					OperationManagerName = b.OperationsManagerFullName
				}).ToList();

			//foreach (BuildingGridViewModel building in buildings.Payload)
			//{
			//    results.Add(new BuildingCsvViewModel()
			//    {
			//        Id = building.ID,
			//        Address = building.FullAddress,
			//        Code = building.Code,
			//        CustomerCode = building.CustomerCode,
			//        EmergencyPhone = building.EmergencyPhone,
			//        IsActive = building.IsActive,
			//        IsComplete = building.IsComplete,
			//        Name = building.Name,
			//        OperationManagerName = building.OperationsManagerFullName
			//    });
			//}
			return results;
		}

		public Task<IEnumerable<BuildingOperationManagerGridViewModel>> ReadAllSharedBuildingsFromOperationsManagerDapperAsync(DataSourceRequest request, int currentOperationsManager, int? operationsManager)
		{
			return this.Repository.GetSharedBuildingsFromOperationsManagerDapperAsync(request, this.CompanyId, currentOperationsManager, operationsManager);
		}

		public async Task UpdateSharedBuildingOperationManager(BuildingUpdateSharedBuildingsEmployeeViewModel vm)
		{
			try
			{
				var buildingEmployee = await this._buildingEmployeesRepository
					.SingleOrDefaultAsync(
						b => b.BuildingId == vm.BuildingId
						&& b.EmployeeId == vm.EmployeeId
						&& b.Type == vm.Type);

				if (buildingEmployee != null)
				{
					if (!vm.IsShared)
					{
						await this._buildingEmployeesRepository.RemoveAsync(buildingEmployee);
						await this._buildingEmployeesRepository.SaveChangesAsync();
					}
				}
				else
				{
					if (vm.IsShared) {
						await this._buildingEmployeesRepository.AddAsync(new BuildingEmployee()
						{
							BuildingId = vm.BuildingId,
							EmployeeId = vm.EmployeeId,
							Type = vm.Type
						});
						await this._buildingEmployeesRepository.SaveChangesAsync();
					}
				}				
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		#region Activity Log
		public Task<DataSource<BuildingActivityLogGridViewModel>> ReadAllActivityLogAsync(DataSourceRequest request, int buildingId, int activityType = -1)
		{
			return this.ActivityLogRepository.ReadAllDapperAsync(request, buildingId, activityType);
		}

		private BuildingActivityLog RegisterLogActivity(
			int buildingId,
			BuildingActivityType activityType,
			IList<ChangeLogEntry> changeLog = null)
		{
			var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

			BuildingActivityLog newLog = new BuildingActivityLog()
			{
				EmployeeId = employeeId,
				BuildingId = buildingId,
				ActivityType = activityType,
				ChangeLog = changeLog
			};

			return this.ActivityLogRepository.Add(newLog);
		}

		private void LogContractActivity(EntityEntry entry)
		{
			int buildingId = -1;

			if (entry.Entity is AuditableEntity<int> entity)
				buildingId = entity.ID;

			var unwantedFields = new List<string> { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
			var modifiedProperties = entry.Properties
											?.Where(p => p.IsModified && !unwantedFields.Contains(p.Metadata.Name))
											?.ToList() ?? new List<PropertyEntry>();

			var changeLog = new List<ChangeLogEntry>();
			try
			{
				foreach (var property in modifiedProperties)
				{
					if (property.OriginalValue == null && property.CurrentValue == null)
						continue;

					var equalsFlag = property.OriginalValue?.Equals(property.CurrentValue) ?? false;
					if (equalsFlag == false)
					{
						string propertyName = property.Metadata.Name;
						string originalValue = string.Empty;
						string currentValue = string.Empty;

						originalValue = property.OriginalValue?.ToString();
						currentValue = property.CurrentValue?.ToString();

						if (propertyName == "IsActive")
						{
							originalValue = property.OriginalValue == null ? "" : (((bool)property.OriginalValue) == true ? "Active" : "Not Active");
							currentValue = property.CurrentValue == null ? "" : (((bool)property.CurrentValue) == true ? "Active" : "Not Active");
						}

						// Ensures we don't display anything (.+)Id like
						if (propertyName.EndsWith("Id"))
						{
							propertyName = propertyName.Substring(0, propertyName.Length - 2);
						}

						changeLog.Add(new ChangeLogEntry
						{
							PropertyName = propertyName,
							OriginalValue = originalValue,
							CurrentValue = currentValue
						});
					}
				}
			}
			catch (Exception ex)
			{
			}

			// Add new log instance
			if (changeLog.Any())
			{
				this.RegisterLogActivity(
					buildingId,
					BuildingActivityType.FieldUpdated,
					changeLog: changeLog);
			}
		}

		protected override void UpdateAuditableEntities()
		{
			var buildingEntries = this.DbContext
								.ChangeTracker
								.Entries()
								.Where(x => (x.Entity is Building) &&
									   (x.State == EntityState.Added ||
										x.State == EntityState.Modified))
								 .ToList(); // Gets a new instance since we are going to modify enumeration

			// Here should comes all log and notifications engines
			// in order to keep all functionality in one place
			foreach (var entry in buildingEntries)
			{
				// Activity log related operations
				if (entry.State == EntityState.Modified)
				{
					this.LogContractActivity(entry);
				}
			}

			base.UpdateAuditableEntities();
		}
		#endregion
	}
}
