
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using MGCap.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MGCap.Domain.ViewModels.ContractActivityLog;
using Newtonsoft.Json;
using MGCap.Domain.ViewModels.ContractNote;
using MGCap.Domain.ViewModels.ContractActivityLogNote;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ContractsApplicationService : BaseSessionApplicationService<Contract, int>, IContractsApplicationService
    {

        public new IContractsRepository Repository => base.Repository as IContractsRepository;
        private readonly IContractItemsRepository ContractItemsRepository;
        private readonly IContractExpensesRepository _ContractExpensesRepository;
        private readonly IOfficeServiceTypesRepository _OfficeServiceTypesRepository;
        private readonly IExpenseSubcategoriesRepository _ExpenseSubcategoriesRepository;
        private readonly IContractOfficeSpacesRepository _ContractOfficeSpacesRepository;
        private readonly IRevenuesRepository _RevenuesRepository;
        private readonly IExpensesRepository _ExpensesRepository;

        private readonly IBuildingsRepository BuildingsRepository;
        private readonly IBuildingActivityLogRepository BuildingActivityLogRepository;
        private readonly ICustomersRepository CustomersRepository;

        private readonly IContractActivityLogRepository ActivityLogRepository;
        private IContractActivityLogNotesRepository _ContractActivityLogNotesRepository;
        private readonly IEmployeesRepository EmployeesRepository;
        private readonly IContractNoteRepository _ContractNoteRepository;
        private IPDFGeneratorApplicationService PDFGeneratorApplicationService { get; set; }

        public ContractsApplicationService(
            IContractsRepository repository,
            IContractItemsRepository contractItemsRepository,
            IContractExpensesRepository contractExpensesRepository,
            IOfficeServiceTypesRepository officeServiceTypesRepository,
            IExpenseSubcategoriesRepository expenseSubcategoriesRepository,
            IContractOfficeSpacesRepository contractOfficeSpacesRepository,
            IRevenuesRepository revenuesRepository,
            IExpensesRepository expensesRepository,
            IBuildingsRepository buildingsRepository,
            IBuildingActivityLogRepository buildingActivityLogRepository,
            ICustomersRepository customersRepository,
            IEmployeesRepository employeesRepository,
            IContractActivityLogRepository activityLogRepository,
            IContractActivityLogNotesRepository contractActivityLogNotesRepository,
            IContractNoteRepository contractNoteRepository,
            IPDFGeneratorApplicationService pDFGeneratorApplicationService,
        IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            this.ContractItemsRepository = contractItemsRepository;
            this._ContractExpensesRepository = contractExpensesRepository;
            this._OfficeServiceTypesRepository = officeServiceTypesRepository;
            this._ExpenseSubcategoriesRepository = expenseSubcategoriesRepository;
            this._ContractOfficeSpacesRepository = contractOfficeSpacesRepository;
            this._RevenuesRepository = revenuesRepository;
            this._ExpensesRepository = expensesRepository;
            this.BuildingsRepository = buildingsRepository;
            this.BuildingActivityLogRepository = buildingActivityLogRepository;
            this.CustomersRepository = customersRepository;
            this.ActivityLogRepository = activityLogRepository;
            this.EmployeesRepository = employeesRepository;
            this.PDFGeneratorApplicationService = pDFGeneratorApplicationService;
            this._ContractNoteRepository = contractNoteRepository;
            this._ContractActivityLogNotesRepository = contractActivityLogNotesRepository;
        }

        public async Task<Contract> AddContractAsync(Contract contract)
        {
            contract.CompanyId = this.CompanyId;
            var newContract = await this.Repository.AddAsync(contract);

            this.RegisterBuildingLogActivity(
                contract.BuildingId,
                BuildingActivityType.Contract,
                new List<ChangeLogEntry>()
                {
                    new ChangeLogEntry()
                    {
                        PropertyName = "Assigned contract",
                        CurrentValue = $" Contract Number {contract.ContractNumber}",
                        OriginalValue = ""
                    }
                });

            return newContract;
        }
        public Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId, id);
        }
        public Task<DataSource<ContractGridViewModel>> ReadAllDapperAsync(DataSourceRequestBudget request, int? status)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId, status);
        }
        public async Task<IEnumerable<ContractExportCsvViewModel>> ReadAllToCsvAsync(DataSourceRequestBudget request, int? status)
        {
            try
            {
                var dataSource = await this.Repository.ReadAllCsvDapperAsync(request, this.CompanyId, status);
                return dataSource.Payload;
            }
            catch (Exception ex)
            {
                return new List<ContractExportCsvViewModel>();
            }
        }
        public Task<Contract> SingleOrDefaultContractByBuildingAsync(int buildingId)
        {
            return this.Repository.SingleOrDefaultContractByBuildingAsync(buildingId);
        }
        public Task<ContractTrackingDetailViewModel> GetContractTrackingDetailsDapperAsync(DataSourceRequest request, int? id, Guid? guid)
        {
            return this.Repository.GetContractTrackingDetailsDapperAsync(request, id, guid);
        }
        public Task<DataSource<Domain.ViewModels.ContractItem.ContractItemGridViewModel>> ReadAllContractItemsDapperAsync(DataSourceRequest request, int contractId)
        {
            return this.ContractItemsRepository.ReadAllDapperAsync(request, contractId);
        }
        public async Task<bool> VerifyContractNumberExists(string contractNumber, int contractId = -1)
        {
            var results = await this.Repository.ReadAllAsync(c => c.ContractNumber == contractNumber && c.ID != contractId);
            return results.Count() > 0 ? true : false;
        }
        public Task<ContractSummaryViewModel> GetContractSummaryAsync(int id)
        {
            return this.Repository.GetContractSummaryDapperAsync(id);
        }
        public async Task DeleteContractAsync(int id)
        {
            await this._RevenuesRepository.RemoveAsync(r => r.ContractId == id);

            await this._ExpensesRepository.RemoveAsync(e => e.ContractId == id);

            await this.ContractItemsRepository.RemoveAsync(i => i.ContractId == id);

            await this._ContractExpensesRepository.RemoveAsync(i => i.ContractId == id);

            await this.ActivityLogRepository.RemoveAsync(l => l.ContractId == id);

            var contract = await this.Repository.SingleOrDefaultAsync(c => c.ID == id);
            await this.Repository.RemoveAsync(contract);

            this.RegisterBuildingLogActivity(
                contract.BuildingId,
                BuildingActivityType.Contract,
                new List<ChangeLogEntry>()
                {
                    new ChangeLogEntry()
                    {
                        PropertyName = "Assigned contract deleted",
                        CurrentValue = $" Contract Number {contract.ContractNumber}",
                        OriginalValue = ""
                    }
                });
        }
        public async Task DisableBuildingContracts(int buildingId)
        {
            var activeContracts = await this.Repository.ReadAllAsync(c => c.BuildingId == buildingId);
            foreach (var contract in activeContracts)
            {
                contract.Status = (int)ContractStatus.Finished;

                await this.Repository.UpdateAsync(contract);
            }
        }
        public async Task AddDefaultItemsAndExpensesToContract(int contractId)
        {
            var contract = await this.Repository.SingleOrDefaultAsync(c => c.ID == contractId);
            var officeSpaces = await this._ContractOfficeSpacesRepository.ReadAllAsync(o => o.ContractId == contractId);

            double totalSquareFeet = officeSpaces.Sum(o => o.SquareFeet);
            int numberWorkers = contract.ProductionRate <= 0 ? 0 : (int)Math.Round((totalSquareFeet / contract.ProductionRate) / 8, MidpointRounding.AwayFromZero);
            int numberSupervisors = numberWorkers < 8 ? 0 : (int)Math.Round((double)(numberWorkers / 8), MidpointRounding.AwayFromZero);
            double adminHours = ((totalSquareFeet * 0.0016) / contract.DaysPerMonth) / 12;
            double vanCrewHours = ((totalSquareFeet * 0.00151) / contract.DaysPerMonth) / 12;

            var log = new List<ItemLogEntry>();

            IEnumerable<ExpenseSubcategory> workerCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Worker");
            ContractExpense worker = new ContractExpense()
            {
                ID = 0,
                Quantity = numberWorkers,
                Description = "Worker",
                ContractId = contract.ID,
                ExpenseCategory = (int)ExpenseCategory.Labor,
                ExpenseTypeName = "Labor",
                ExpenseSubcategoryId = workerCategory.Count() == 0 ? 0 : workerCategory.First().ID,
                ExpenseSubcategoryName = workerCategory.Count() == 0 ? "Worker" : workerCategory.First().Name,
                Rate = workerCategory.Count() == 0 ? 0 : workerCategory.First().Rate,
                RateType = workerCategory.Count() == 0 ? (int)ExpenseRateType.Hour : workerCategory.First().RateType,
                Value = 8,
                RatePeriodicity = workerCategory.Count() == 0 ? "Daily" : workerCategory.First().Periodicity,
                DefaultType = ContractExpenseDefaultType.Worker
            };
            await this._ContractExpensesRepository.AddAsync(worker);
            log.Add(new ItemLogEntry()
            {
                ItemType = 1,
                ActivityType = "Added (Pre Populated)",
                Value = $"Estimated Expense ({worker.ExpenseSubcategoryName} - ${worker.Rate.ToString("N2")})"
            });

            IEnumerable<ExpenseSubcategory> supervisorCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Supervisor");
            ContractExpense supervisor = new ContractExpense()
            {
                ID = 0,
                ContractId = contract.ID,
                Quantity = numberSupervisors,
                Description = "Supervisor",
                ExpenseCategory = (int)ExpenseCategory.Labor,
                ExpenseTypeName = "Labor",
                ExpenseSubcategoryId = supervisorCategory.Count() == 0 ? 0 : supervisorCategory.First().ID,
                ExpenseSubcategoryName = supervisorCategory.Count() == 0 ? "Supervisor" : supervisorCategory.First().Name,
                Rate = supervisorCategory.Count() == 0 ? 0 : supervisorCategory.First().Rate,
                RateType = supervisorCategory.Count() == 0 ? (int)ExpenseRateType.Hour : supervisorCategory.First().RateType,
                Value = 8,
                RatePeriodicity = supervisorCategory.Count() == 0 ? "Daily" : supervisorCategory.First().Periodicity,
                DefaultType = ContractExpenseDefaultType.Supervisor
            };
            await this._ContractExpensesRepository.AddAsync(supervisor);
            log.Add(new ItemLogEntry()
            {
                ItemType = 1,
                ActivityType = "Added (Pre Populated)",
                Value = $"Estimated Expense ({supervisor.ExpenseSubcategoryName} - ${supervisor.Rate.ToString("N2")})"
            });

            IEnumerable<ExpenseSubcategory> dayporterCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Dayporter");
            ContractExpense dayporter = new ContractExpense()
            {
                ID = 0,
                ContractId = contract.ID,
                Quantity = 1,
                Description = "Dayporter",
                ExpenseCategory = (int)ExpenseCategory.Labor,
                ExpenseTypeName = "Labor",
                ExpenseSubcategoryId = dayporterCategory.Count() == 0 ? 0 : dayporterCategory.First().ID,
                ExpenseSubcategoryName = dayporterCategory.Count() == 0 ? "Dayporter" : dayporterCategory.First().Name,
                Rate = dayporterCategory.Count() == 0 ? 0 : dayporterCategory.First().Rate,
                RateType = dayporterCategory.Count() == 0 ? (int)ExpenseRateType.Hour : dayporterCategory.First().RateType,
                Value = 8,
                RatePeriodicity = dayporterCategory.Count() == 0 ? "Daily" : dayporterCategory.First().Periodicity,
                DefaultType = ContractExpenseDefaultType.Dayporter
            };
            await this._ContractExpensesRepository.AddAsync(dayporter);
            log.Add(new ItemLogEntry()
            {
                ItemType = 1,
                ActivityType = "Added (Pre Populated)",
                Value = $"Estimated Expense ({dayporter.ExpenseSubcategoryName} - ${dayporter.Rate.ToString("N2")})"
            });

            IEnumerable<ExpenseSubcategory> adminCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Admin/Operations");
            ContractExpense adminOperations = new ContractExpense()
            {
                ID = 0,
                ContractId = contract.ID,
                Quantity = 1,
                Description = "Admin/Operations",
                ExpenseCategory = (int)ExpenseCategory.Labor,
                ExpenseTypeName = "Labor",
                ExpenseSubcategoryId = adminCategory.Count() == 0 ? 0 : adminCategory.First().ID,
                ExpenseSubcategoryName = adminCategory.Count() == 0 ? "Admin/Operations" : adminCategory.First().Name,
                Rate = adminCategory.Count() == 0 ? 0 : adminCategory.First().Rate,
                RateType = adminCategory.Count() == 0 ? (int)ExpenseRateType.Hour : adminCategory.First().RateType,
                Value = adminHours,
                RatePeriodicity = adminCategory.Count() == 0 ? "Daily" : adminCategory.First().Periodicity,
                DefaultType = ContractExpenseDefaultType.AdminOperations
            };
            await this._ContractExpensesRepository.AddAsync(adminOperations);
            log.Add(new ItemLogEntry()
            {
                ItemType = 1,
                ActivityType = "Added (Pre Populated)",
                Value = $"Estimated Expense ({adminOperations.ExpenseSubcategoryName} - ${adminOperations.Rate.ToString("N2")})"
            });

            IEnumerable<ExpenseSubcategory> vanCrewCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Van Crew");
            ContractExpense vanCrew = new ContractExpense()
            {
                ID = 0,
                ContractId = contract.ID,
                Quantity = 1,
                Description = "Van Crew",
                ExpenseCategory = (int)ExpenseCategory.Labor,
                ExpenseTypeName = "Labor",
                ExpenseSubcategoryId = vanCrewCategory.Count() == 0 ? 0 : vanCrewCategory.First().ID,
                ExpenseSubcategoryName = vanCrewCategory.Count() == 0 ? "Van Crew" : vanCrewCategory.First().Name,
                Rate = vanCrewCategory.Count() == 0 ? 0 : vanCrewCategory.First().Rate,
                RateType = vanCrewCategory.Count() == 0 ? (int)ExpenseRateType.Hour : vanCrewCategory.First().RateType,
                Value = vanCrewHours,
                RatePeriodicity = vanCrewCategory.Count() == 0 ? "Daily" : vanCrewCategory.First().Periodicity,
                DefaultType = ContractExpenseDefaultType.VanCrew
            };
            await this._ContractExpensesRepository.AddAsync(vanCrew);
            log.Add(new ItemLogEntry()
            {
                ItemType = 1,
                ActivityType = "Added (Pre Populated)",
                Value = $"Estimated Expense ({vanCrew.ExpenseSubcategoryName} - ${vanCrew.Rate.ToString("N2")})"
            });

            IEnumerable<ExpenseSubcategory> supplyCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Supplies");
            double? sqft = null;
            foreach (ContractOfficeSpace space in officeSpaces)
            {
                var officeServiceType = await this._OfficeServiceTypesRepository.SingleOrDefaultAsync(o => o.ID == space.OfficeTypeId);

                ContractItem cItem = new ContractItem()
                {
                    ID = 0,
                    ContractId = contract.ID,
                    Quantity = 1,
                    Description = officeServiceType.Name,
                    OfficeServiceTypeId = officeServiceType.ID,
                    OfficeServiceTypeName = officeServiceType.Name,
                    Rate = officeServiceType.Rate,
                    RateType = officeServiceType.RateType,
                    RatePeriodicity = officeServiceType.Periodicity,
                    SquareFeet = space.SquareFeet,
                    DefaultType = ContractItemDefaultType.OfficeSpace
                };
                await this.ContractItemsRepository.AddAsync(cItem);
                log.Add(new ItemLogEntry()
                {
                    ItemType = 0,
                    ActivityType = "Added (Pre Populated)",
                    Value = $"Estimated Revenue ({cItem.OfficeServiceTypeName} - ${cItem.Rate.ToString("N2")})"
                });

                sqft = sqft.HasValue ? (sqft.Value + space.SquareFeet) : space.SquareFeet;

                ContractExpense cExpense = new ContractExpense()
                {
                    ID = 0,
                    ContractId = contract.ID,
                    Quantity = 1,
                    Description = $"Supplies {officeServiceType.Name}",
                    ExpenseCategory = (int)ExpenseCategory.Supplies,
                    ExpenseTypeName = "Supplies",
                    ExpenseSubcategoryId = supplyCategory.Count() == 0 ? 0 : supplyCategory.First().ID,
                    ExpenseSubcategoryName = supplyCategory.Count() == 0 ? "Supplies" : supplyCategory.First().Name,
                    Rate = supplyCategory.Count() == 0 ? 0 : supplyCategory.First().Rate,
                    RateType = supplyCategory.Count() == 0 ? (int)ExpenseRateType.Unit : supplyCategory.First().RateType,
                    Value = 1,
                    RatePeriodicity = supplyCategory.Count() == 0 ? "Monthly" : supplyCategory.First().Periodicity,
                    DefaultType = ContractExpenseDefaultType.Supplies
                };
                await this._ContractExpensesRepository.AddAsync(cExpense);
                log.Add(new ItemLogEntry()
                {
                    ItemType = 1,
                    ActivityType = "Added (Pre Populated)",
                    Value = $"Estimated Expense ({cExpense.ExpenseSubcategoryName} - ${cExpense.Rate.ToString("N2")})"
                });
            }

            this.RegisterLogActivity(
                    contractId,
                    ContractActivityType.ItemUpdated, null,
                    log);

            if (sqft.HasValue)
            {
                await this.RegisterContractSquareFeetUpdated(contractId, sqft.Value, "Occupied");
            }
        }
        public async Task UpdateDefaultItemsAndExpensesToContract(int contractId)
        {
            var contract = await this.Repository.SingleOrDefaultAsync(c => c.ID == contractId);

            // var officeSpaces = await this._ContractOfficeSpacesRepository.ReadAllAsync(o => o.ContractId == contractId);

            // var officeSpaces = await this.ContractItemsRepository.ReadAllSquareFeetByTypeDapperAsync(contractId);

            var officeSpaces = await this.ContractItemsRepository.ReadAllAsync(i => i.ContractId == contractId);

            var contractItems = await this.ContractItemsRepository.ReadAllAsync(i => i.ContractId == contractId && i.DefaultType > ContractItemDefaultType.None);
            var contractExpenses = await this._ContractExpensesRepository.ReadAllAsync(e => e.ContractId == contractId && e.DefaultType > ContractExpenseDefaultType.None);

            double totalSquareFeet = officeSpaces.Sum(o => o.SquareFeet.HasValue ? o.SquareFeet.Value : 0);
            int numberWorkers = contract.ProductionRate <= 0 ? 0 : (int)Math.Round((totalSquareFeet / contract.ProductionRate) / 8, MidpointRounding.AwayFromZero);
            int numberSupervisors = numberWorkers < 8 ? 0 : (int)Math.Round((double)(numberWorkers / 8), MidpointRounding.AwayFromZero);
            double adminHours = ((totalSquareFeet * 0.0016) / contract.DaysPerMonth) / 12;
            double vanCrewHours = ((totalSquareFeet * 0.00151) / contract.DaysPerMonth) / 12;

            var log = new List<ItemLogEntry>();

            ContractExpense worker = contractExpenses.SingleOrDefault(e => e.DefaultType == ContractExpenseDefaultType.Worker);
            if (worker != null)
            {
                worker.Quantity = numberWorkers;
                await this._ContractExpensesRepository.UpdateAsync(worker);
                log.Add(
                    new ItemLogEntry()
                    {
                        ItemType = 1,
                        ActivityType = "Updated (Pre Populated)",
                        Value = $"Estimated Expense ({worker.ExpenseSubcategoryName} - ${worker.Rate.ToString("N2")})"
                    });
            }

            ContractExpense supervisor = contractExpenses.SingleOrDefault(e => e.DefaultType == ContractExpenseDefaultType.Supervisor);
            if (supervisor != null)
            {
                supervisor.Quantity = numberSupervisors;
                await this._ContractExpensesRepository.UpdateAsync(supervisor);
                log.Add(
                    new ItemLogEntry()
                    {
                        ItemType = 1,
                        ActivityType = "Updated (Pre Populated)",
                        Value = $"Estimated Expense ({supervisor.ExpenseSubcategoryName} - ${supervisor.Rate.ToString("N2")})"
                    });
            }

            ContractExpense adminOperations = contractExpenses.SingleOrDefault(e => e.DefaultType == ContractExpenseDefaultType.AdminOperations);
            if (adminOperations != null)
            {
                adminOperations.Value = adminHours;
                await this._ContractExpensesRepository.UpdateAsync(adminOperations);
                log.Add(
                    new ItemLogEntry()
                    {
                        ItemType = 1,
                        ActivityType = "Updated (Pre Populated)",
                        Value = $"Estimated Expense ({adminOperations.ExpenseSubcategoryName} - ${adminOperations.Rate.ToString("N2")})"
                    });
            }

            ContractExpense vanCrew = contractExpenses.SingleOrDefault(e => e.DefaultType == ContractExpenseDefaultType.VanCrew);
            if (vanCrew != null)
            {
                vanCrew.Value = vanCrewHours;
                await this._ContractExpensesRepository.UpdateAsync(vanCrew);
                log.Add(
                    new ItemLogEntry()
                    {
                        ItemType = 1,
                        ActivityType = "Updated (Pre Populated)",
                        Value = $"Estimated Expense ({vanCrew.ExpenseSubcategoryName} - ${vanCrew.Rate.ToString("N2")})"
                    });
            }

            if (log.Count() > 0)
            {
                this.RegisterLogActivity(contractId, ContractActivityType.ItemUpdated, null, log);
            }

            var offices = await this.ContractItemsRepository.ReadAllSquareFeetByTypeDapperAsync(contractId);
            await this._ContractExpensesRepository.RemoveAsync(i => i.DefaultType == ContractExpenseDefaultType.Supplies);
            IEnumerable<ExpenseSubcategory> supplyCategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, "Supplies");
            foreach (ContractOfficeSpaceGridViewModel office in offices)
            {
                ContractExpense cExpense = new ContractExpense()
                {
                    ID = 0,
                    ContractId = contract.ID,
                    Quantity = 1,
                    Description = $"Supplies {office.OfficeTypeName}",
                    ExpenseCategory = (int)ExpenseCategory.Supplies,
                    ExpenseTypeName = "Supplies",
                    ExpenseSubcategoryId = supplyCategory.Count() == 0 ? 0 : supplyCategory.First().ID,
                    ExpenseSubcategoryName = supplyCategory.Count() == 0 ? "Supplies" : supplyCategory.First().Name,
                    Rate = supplyCategory.Count() == 0 ? 0 : supplyCategory.First().Rate,
                    RateType = supplyCategory.Count() == 0 ? (int)ExpenseRateType.Unit : supplyCategory.First().RateType,
                    Value = 1,
                    RatePeriodicity = supplyCategory.Count() == 0 ? "Monthly" : supplyCategory.First().Periodicity,
                    DefaultType = ContractExpenseDefaultType.Supplies
                };

                await this._ContractExpensesRepository.AddAsync(cExpense);
            }
        }
        protected override void UpdateAuditableEntities()
        {
            var contractEntries = this.DbContext
                                .ChangeTracker
                                .Entries()
                                .Where(x => (x.Entity is Contract) &&
                                       (x.State == EntityState.Added ||
                                        x.State == EntityState.Modified))
                                 .ToList(); // Gets a new instance since we are going to modify enumeration

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place
            foreach (var entry in contractEntries)
            {
                // Activity log related operations
                if (entry.State == EntityState.Modified)
                {
                    this.LogContractActivity(entry);
                }
            }

            base.UpdateAuditableEntities();
        }
        public async Task<DataSource<ContractActivityLogGridViewModel>> ReadAllActivityLog(DataSourceRequest request, int contractId)
        {
            return await this.ActivityLogRepository.ReadAllDapperAsync(request, contractId);
        }
        public async Task UpdateBudgetProfit(int budgetId)
        {
            var budget = await this.Repository.SingleOrDefaultAsync(c => c.ID == budgetId);
            var revenues = await this.ContractItemsRepository.ReadAllAsync(r => r.ContractId == budgetId);
            var expenses = await this._ContractExpensesRepository.ReadAllAsync(e => e.ContractId == budgetId);

            double[] totalRevenues = this.CalculateBudgetRevenuesProfit(revenues.AsEnumerable(), budget.DaysPerMonth);
            double[] totalExpenses = this.CalculateBudgetExpensesProfit(expenses.AsEnumerable(), budget.DaysPerMonth);

            budget.DailyProfit = (totalRevenues[0] - totalExpenses[0]);
            budget.MonthlyProfit = (totalRevenues[1] - totalExpenses[1]);
            budget.YearlyProfit = (totalRevenues[2] - totalExpenses[2]);

            budget.DailyProfitRatio = (totalRevenues[0] == 0 ? 0 : (budget.DailyProfit / totalRevenues[0]) * 100);
            budget.MonthlyProfitRatio = (totalRevenues[1] == 0 ? 0 : (budget.MonthlyProfit / totalRevenues[1]) * 100);
            budget.YearlyProfitRatio = (totalRevenues[2] == 0 ? 0 : (budget.YearlyProfit / totalRevenues[2]) * 100);

            await this.Repository.UpdateAsync(budget);
        }
        public async Task<string> GetBudgetDocumentUrl(int budgetId)
        {
            var budgetDetail = await this.Repository.GetContractReportDetailsDapperAsync(budgetId, null);

            ContractExportPdfViewModel contractExport = new ContractExportPdfViewModel()
            {
                BuildingName = budgetDetail.BuildingName,
                DaysPerMonth = budgetDetail.DaysPerMonth,
                ContractNumber = budgetDetail.ContractNumber,
                CustomerName = budgetDetail.CustomerName,
                Description = budgetDetail.Description,
                ExpirationDate = budgetDetail.ExpirationDate.HasValue ? budgetDetail.ExpirationDate.Value.ToString("dddd, dd MMMM yyyy") : "",
                NumberOfPeople = budgetDetail.NumberOfPeople,
                UnoccupiedSquareFeets = budgetDetail.UnoccupiedSquareFeets,
                ProductionRate = budgetDetail.ProductionRate,
                Status = budgetDetail.Status.ToString().SplitCamelCase()
            };

            double occupiedSquareFeet = budgetDetail.ContractItems
                .Sum(e => e.SquareFeet.HasValue && e.RateType == (int)ServiceRateType.SquareFeet ? e.SquareFeet.Value : 0);

            contractExport.OccupiedSquareFeets = occupiedSquareFeet;
            contractExport.TotalSquareFeets = occupiedSquareFeet + contractExport.UnoccupiedSquareFeets;

            contractExport.Revenue = budgetDetail.ContractItems
                .Select(r => new ContractItemExportPdfViewModel(
                    r.Description,
                    r.Quantity,
                    r.OfficeServiceTypeName,
                    r.Rate,
                    this.GetContractItemGridValueByType(r),
                    r.RatePeriodicity,
                    budgetDetail.DaysPerMonth
                ));

            contractExport.Expenses = budgetDetail.ContractExpenses
                .Select(e => new ContractExpenseExportPdfViewModel(
                    e.Description,
                    e.Quantity,
                    e.ExpenseTypeName,
                    e.Rate,
                    e.Value,
                    e.RatePeriodicity,
                    e.OverheadPercent,
                    (ExpenseCategory)e.ExpenseCategory,
                    budgetDetail.DaysPerMonth
                ));

            double totalRevenueDaily = contractExport.Revenue.Sum(r => r.DailyRate);
            double totalRevenueMonthly = contractExport.Revenue.Sum(r => r.MonthlyRate);
            double totalRevenueYearly = contractExport.Revenue.Sum(r => r.YearlyRate);

            var laborExpenses = contractExport.Expenses.Where(e => e.ExpenseCategory == ExpenseCategory.Labor);

            contractExport.ExpensesOverheadDaily = laborExpenses.Sum(e => e.DailyRate) * 0.14;
            contractExport.ExpensesOverheadMonthly = laborExpenses.Sum(e => e.MonthlyRate) * 0.14;
            contractExport.ExpensesOverheadYearly = laborExpenses.Sum(e => e.YearlyRate) * 0.14;

            double totalExpensesDaily = contractExport.Expenses.Sum(e => e.DailyRate) + contractExport.ExpensesOverheadDaily;
            double totalExpensesMonthly = contractExport.Expenses.Sum(e => e.MonthlyRate) + contractExport.ExpensesOverheadMonthly;
            double totalExpensesYearly = contractExport.Expenses.Sum(e => e.YearlyRate) + contractExport.ExpensesOverheadYearly;

            contractExport.DailyProfit = totalRevenueDaily - totalExpensesDaily;
            contractExport.MonthlyProfit = totalRevenueMonthly - totalExpensesMonthly;
            contractExport.YearlyProfit = totalRevenueYearly - totalExpensesYearly;

            contractExport.DailyProfitRatio = totalRevenueDaily == 0 ? 0 : (contractExport.DailyProfit / totalRevenueDaily);
            contractExport.MonthlyProfitRatio = totalRevenueMonthly == 0 ? 0 : (contractExport.MonthlyProfit / totalRevenueMonthly);
            contractExport.YearlyProfitRatio = totalRevenueYearly == 0 ? 0 : (contractExport.YearlyProfit / totalRevenueYearly);

            string budgetJson = JsonConvert.SerializeObject(contractExport);

            // TODO: Add document code
            string url = await this.PDFGeneratorApplicationService.GetDocumentUrl("69297", budgetJson);

            return url;
        }
        public Task<ContractChildDetailViewModel> GetBudgetDetail(int? id, Guid? guid)
        {
            return this.Repository.GetBudgetDetailsDapperAsync(id, guid, this.UserEmail);
        }
        public async Task UpdateContractChildrenPeriodicityRate(int contractId, double daysPerMonth)
        {
            var revenues = await this.ContractItemsRepository.ReadAllAsync(r => r.ContractId == contractId);
            if (revenues.Count() > 0)
            {
                foreach (var revenue in revenues)
                {
                    revenue.UpdateContractRevenueRatePeriods(daysPerMonth);
                }
                await this.ContractItemsRepository.UpdateRangeAsync(revenues);
            }

            var expenses = await this._ContractExpensesRepository.ReadAllAsync(e => e.ContractId == contractId);
            if (expenses.Count() > 0)
            {
                var expensesList = expenses.ToList();
                for (int i = 0; i < expensesList.Count(); i++)
                {
                    expensesList[i].UpdateContractExpenseRatePeriods(daysPerMonth);
                    await this._ContractExpensesRepository.UpdateAsync(expensesList[i]);
                }
            }

        }

        #region Contract Item        
        public async Task<ContractItem> AddContractItemAsync(ContractItem contractItem)
        {
            this.RegisterLogActivity(
                    contractItem.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 0,
                            ActivityType = "Added",
                            Value = $"Estimated Revenue ({contractItem.OfficeServiceTypeName} - ${contractItem.Rate.ToString("N2")})"
                        }
                    });

            if (contractItem.RateType == ServiceRateType.SquareFeet)
            {
                await this.RegisterContractSquareFeetUpdated(contractItem.ContractId, contractItem.SquareFeet.HasValue ? contractItem.SquareFeet.Value : 0, "Occupied");
            }

            var contract = this.Repository.SingleOrDefault(c => c.ID == contractItem.ContractId);
            if (contract != null)
            {
                contractItem.UpdateContractRevenueRatePeriods(contract.DaysPerMonth);
            }

            return await this.ContractItemsRepository.AddAsync(contractItem);
        }
        public async Task<ContractItem> UpdateContractItemAsync(ContractItem contractItem)
        {
            if (contractItem.DefaultType == ContractItemDefaultType.OfficeSpace)
            {
                contractItem.DefaultType = ContractItemDefaultType.None;
            }

            this.RegisterLogActivity(
                    contractItem.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 0,
                            ActivityType = "Updated",
                            Value = $"Estimated Revenue ({contractItem.OfficeServiceTypeName} - ${contractItem.Rate.ToString("N2")})"
                        }
                    });

            if (contractItem.RateType == ServiceRateType.SquareFeet)
            {
                await this.RegisterContractSquareFeetUpdated(contractItem.ContractId, contractItem.SquareFeet.HasValue ? contractItem.SquareFeet.Value : 0, "Occupied", contractItem.ID);
            }

            var contract = this.Repository.SingleOrDefault(c => c.ID == contractItem.ContractId);
            if (contract != null)
            {
                contractItem.UpdateContractRevenueRatePeriods(contract.DaysPerMonth);
            }

            return await this.ContractItemsRepository.UpdateAsync(contractItem);
        }
        public Task<ContractItem> GetContractItemByIdAsync(int id)
        {
            return this.ContractItemsRepository.SingleOrDefaultAsync(i => i.ID == id);
        }
        public async Task RemoveContractItemAsync(ContractItem contractItem)
        {
            this.RegisterLogActivity(
                    contractItem.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 0,
                            ActivityType = "Removed",
                            Value = $"Estimated Revenue ({contractItem.OfficeServiceTypeName}  - ${contractItem.Rate.ToString("N2")})"
                        }
                    });

            if (contractItem.RateType == ServiceRateType.SquareFeet)
            {
                await this.RegisterContractSquareFeetRemoved(contractItem.ContractId, contractItem.SquareFeet.HasValue ? contractItem.SquareFeet.Value : 0);
            }

            await this.ContractItemsRepository.RemoveAsync(contractItem);
        }
        public async Task<ContractHttpCsvResponseViewModel> AddContractItemFromCsvAsync(ContractItemCsvViewModel vm)
        {
            try
            {
                // Validate existing contract number
                var contract = await this.Repository.SingleOrDefaultAsync(c => (string.IsNullOrWhiteSpace(c.ContractNumber) ? c.ID.ToString() : c.ContractNumber) == vm.ContractNumber);

                var Response = new ContractHttpCsvResponseViewModel
                {
                    ErrorCode = 0,
                    Message = "successful"
                };

                if (contract == null)
                {
                    // Contract Number not found
                    Response.ErrorCode = 3;
                    Response.Message = "contractNumber not found";
                    return Response;
                }

                var officeType = await this._OfficeServiceTypesRepository.FindByName(this.CompanyId, vm.OfficeServiceTypeName);
                //IEnumerable<string> validPeriods = new List<string> { "Daily", "Monthly", "Bi-Monthly", "Quarterly", "Bi-Annually", "Yearly" };

                // bool validPeriodsBool = validPeriods.ToList().Any(x => x == vm.RatePeriodicity);
                if (officeType.Count() == 0)
                {
                    Response.ErrorCode = 6;
                    Response.Message = "Office Service Type Name Not Found";
                    return Response;
                }

                try
                {
                    ContractItem cItem = new ContractItem()
                    {
                        ID = 0,
                        ContractId = contract.ID,
                        Quantity = vm.Quantity,
                        Description = vm.Description,
                        OfficeServiceTypeId = officeType.Count() == 0 ? 0 : officeType.First().ID,
                        OfficeServiceTypeName = vm.OfficeServiceTypeName,
                        Rate = vm.Rate,
                        RateType = officeType.First().RateType,
                        RatePeriodicity = officeType.First().Periodicity,
                        Hours = vm.Value,
                        Amount = vm.Value,
                        Rooms = vm.Value,
                        SquareFeet = vm.Value,
                        DefaultType = ContractItemDefaultType.None
                    };

                    await this.ContractItemsRepository.AddAsync(cItem);

                    this.RegisterLogActivity(
                            cItem.ContractId,
                            ContractActivityType.ItemUpdated, null,
                            new List<ItemLogEntry>()
                            {
                                new ItemLogEntry()
                                {
                                    ItemType = 0,
                                    ActivityType = "Added (CSV)",
                                    Value = $"Estimated Revenue ({cItem.OfficeServiceTypeName} - ${cItem.Rate.ToString("N2")})"
                                }
                            });

                    if (cItem.RateType == ServiceRateType.SquareFeet)
                    {
                        await this.RegisterContractSquareFeetUpdated(cItem.ContractId, cItem.SquareFeet.HasValue ? cItem.SquareFeet.Value : 0, "Occupied");
                    }
                }
                catch (Exception)
                {
                    Response.ErrorCode = 6;
                    Response.Message = "Invalid data";
                    return Response;
                }

                return Response;
            }
            catch (Exception ex)
            {
                // General Error
                var Response = new ContractHttpCsvResponseViewModel
                {
                    ErrorCode = 6,
                    Message = "Invalid data"
                };
                return Response;
            }
        }
        public Task<IEnumerable<ContractOfficeSpaceGridViewModel>> ReadAllRevenuesSquareFeetAsync(int contractId)
        {
            return this.ContractItemsRepository.ReadAllSquareFeetByTypeDapperAsync(contractId);
        }
        public async Task UpdateContractItemOrder(ContractItemUpdateOrderViewModel vm)
        {
            var previous = await this.ContractItemsRepository.SingleOrDefaultAsync(r => r.ID == vm.PreviousContractItemId);
            if (previous != null)
            {
                previous.Order = vm.PreviousContractItemOrder;
            }
            var next = await this.ContractItemsRepository.SingleOrDefaultAsync(r => r.ID == vm.NextContractItemId);
            if (next != null)
            {
                next.Order = vm.NextContractItemOrder;
            }

            if (previous != null && next != null)
            {
                var items = new List<ContractItem>() { previous, next };
                await this.ContractItemsRepository.UpdateRangeAsync(items.AsEnumerable());
            }
        }
        #endregion

        #region Contract Expense       
        public Task<ContractExpense> AddContractExpenseAsync(ContractExpense contractExpense)
        {
            this.RegisterLogActivity(
                    contractExpense.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 1,
                            ActivityType = "Added",
                            Value = $"Estimated Expense ({contractExpense.ExpenseSubcategoryName} - ${contractExpense.Rate.ToString("N2")})"
                        }
                    });

            var contract = this.Repository.SingleOrDefault(c => c.ID == contractExpense.ContractId);
            if (contract != null)
            {
                contractExpense.UpdateContractExpenseRatePeriods(contract.DaysPerMonth);
            }
            return this._ContractExpensesRepository.AddAsync(contractExpense);
        }
        public Task<ContractExpense> UpdateContractExpenseAsync(ContractExpense contractExpense)
        {
            this.RegisterLogActivity(
                    contractExpense.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 1,
                            ActivityType = "Updated",
                            Value = $"Estimated Expense ({contractExpense.ExpenseSubcategoryName} - ${contractExpense.Rate.ToString("N2")})"
                        }
                    });

            var contract = this.Repository.SingleOrDefault(c => c.ID == contractExpense.ContractId);
            if (contract != null)
            {
                contractExpense.UpdateContractExpenseRatePeriods(contract.DaysPerMonth);
            }
            return this._ContractExpensesRepository.UpdateAsync(contractExpense);
        }
        public Task<ContractExpense> GetContractExpenseByIdAsync(int id)
        {
            return this._ContractExpensesRepository.SingleOrDefaultAsync(e => e.ID == id);
        }
        public Task<DataSource<ContractExpenseGridViewModel>> ReadAllContractExpensesDapperAsync(DataSourceRequest request, int contractId)
        {
            return this._ContractExpensesRepository.ReadAllDapperAsync(request, contractId);
        }
        public Task RemoveContractExpenseAsync(ContractExpense contractExpense)
        {
            this.RegisterLogActivity(
                    contractExpense.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 1,
                            ActivityType = "Removed",
                            Value = $"Estimated Expense ({contractExpense.ExpenseSubcategoryName} - ${contractExpense.Rate.ToString("N2")})"
                        }
                    });

            return this._ContractExpensesRepository.RemoveAsync(contractExpense);
        }
        public async Task<ContractHttpCsvResponseViewModel> AddContractExpenseFromCsv(ContractExpenseCsvViewModel vm)
        {
            var Response = new ContractHttpCsvResponseViewModel
            {
                ErrorCode = 0,
                Message = "successful"
            };
            try
            {
                // Validate existing contract number
                var contract = await this.Repository.SingleOrDefaultAsync(c => (string.IsNullOrWhiteSpace(c.ContractNumber) ? c.ID.ToString() : c.ContractNumber) == vm.ContractNumber);

                if (contract == null)
                {
                    // Contract Number not found
                    Response.ErrorCode = 3;
                    Response.Message = "ContractCode not found";
                    return Response;
                }

                //Validate ExpenseType

                var expennseType = FindExpenseCategoryByName(vm.ExpenseType).ToString();
                if (expennseType != vm.ExpenseType)
                {
                    Response.ErrorCode = 6;
                    Response.Message = "Expense Type not found";
                    return Response;
                }

                var subcategory = await this._ExpenseSubcategoriesRepository.FindByName(this.CompanyId, vm.ExpenseSubcategory);
                if (subcategory.Count() == 0)
                {
                    Response.ErrorCode = 6;
                    Response.Message = "Expense Subcategory not found";
                    return Response;
                }

                ContractExpense worker = new ContractExpense()
                {
                    ID = 0,
                    Quantity = vm.Quantity,
                    Description = vm.Description,
                    ContractId = contract.ID,
                    ExpenseCategory = (int)FindExpenseCategoryByName(vm.ExpenseType),
                    ExpenseTypeName = FindExpenseCategoryByName(vm.ExpenseType).ToString(),
                    ExpenseSubcategoryId = subcategory.Count() == 0 ? 0 : subcategory.First().ID,
                    ExpenseSubcategoryName = subcategory.First().Name,
                    Rate = vm.Rate,
                    RateType = subcategory.First().RateType,
                    Value = vm.Value,
                    RatePeriodicity = subcategory.First().Periodicity,
                    DefaultType = ContractExpenseDefaultType.None
                };
                await this._ContractExpensesRepository.AddAsync(worker);

                this.RegisterLogActivity(
                    worker.ContractId,
                    ContractActivityType.ItemUpdated, null,
                    new List<ItemLogEntry>()
                    {
                        new ItemLogEntry()
                        {
                            ItemType = 1,
                            ActivityType = "Added (CSV)",
                            Value = $"Estimated Expense ({worker.ExpenseSubcategoryName} - ${worker.Rate.ToString("N2")})"
                        }
                    });

                // success
                return Response;
            }
            catch (Exception ex)
            {
                // General Error
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return Response;
            }
        }
        private ExpenseCategory FindExpenseCategoryByName(string name)
        {
            ExpenseCategory category = ExpenseCategory.Others;
            switch (name)
            {
                case "Labor":
                    category = ExpenseCategory.Labor;
                    break;
                case "Equipments":
                    category = ExpenseCategory.Equipments;
                    break;
                case "Supplies":
                    category = ExpenseCategory.Supplies;
                    break;
                case "Others":
                    category = ExpenseCategory.Others;
                    break;
                case "Subcontractor":
                    category = ExpenseCategory.Subcontractor;
                    break;
                default:
                    category = ExpenseCategory.Labor;
                    break;
            }
            return category;
        }
        #endregion

        public Task<ContractReportDetailViewModel> GetContractReportDetailsDapperAsync(int? contractId, Guid? guid)
        {
            return this.Repository.GetContractReportDetailsDapperAsync(contractId, guid);
        }

        public async Task<DataSource<ContractListBoxViewModel>> ReadAllCboByBuildingDapperAsync(int? buildingId, int? customerId, DateTime? date, int? contractId = null)
        {
            return await this.Repository.ReadAllCboByBuildingDapperAsync(this.CompanyId, buildingId, customerId, date, contractId);
        }

        public Task<ContractReportDetailViewModel> GetContractReportDetailsBalancesDapperAsync(int id, Guid? contractId, DataSourceRequest request)
        {
            return this.Repository.GetContractReportDetailsBalancesDapperAsync(id, contractId, request);
        }

        public Task<DataSource<ContractOfficeSpaceGridViewModel>> ReadAllOfficeSpacesDapperAsync(DataSourceRequest request, int contractId)
        {
            return this._ContractOfficeSpacesRepository.ReadAllDapperAsync(request, contractId);
        }
        public Task<ContractOfficeSpace> AddOfficeSpaceAsync(ContractOfficeSpace officeSpace)
        {
            return this._ContractOfficeSpacesRepository.AddAsync(officeSpace);
        }
        public Task<ContractOfficeSpace> GetOfficeSpaceByIdAsync(int officeSpaceId)
        {
            return this._ContractOfficeSpacesRepository.SingleOrDefaultAsync(o => o.ID == officeSpaceId);
        }
        public Task<ContractOfficeSpace> UpdateOfficeSpaceAsync(ContractOfficeSpace officeSpace)
        {
            return this._ContractOfficeSpacesRepository.UpdateAsync(officeSpace);
        }

        public async Task<ContractHttpCsvResponseViewModel> AddContractFromCsvAsync(ContractCreateViewModel expenseVm)
        {
            var Response = new ContractHttpCsvResponseViewModel
            {
                ErrorCode = 0,
                Message = "successful"
            };

            // Buscar customer por el CustomerCode
            var customer = await CustomersRepository.SingleOrDefaultAsync(c => (string.IsNullOrWhiteSpace(c.Code) ? c.ID.ToString() : c.Code).Equals(expenseVm.CustomerCode) && c.CompanyId == this.CompanyId);

            // No se encontro customer
            if (customer == null)
            {
                Response.ErrorCode = 1;
                Response.Message = "CustomerCode not found";
                return Response;
            }

            // Buscar building por el BuildingCode
            var building = await BuildingsRepository.SingleOrDefaultAsync(b => (string.IsNullOrWhiteSpace(b.Code) ? b.ID.ToString() : b.Code).Equals(expenseVm.BuildingCode) && b.CompanyId == this.CompanyId);

            // No se encontro contrato
            if (building == null)
            {
                Response.ErrorCode = 2;
                Response.Message = "BuildingCode not found";
                return Response;
            }

            var activeContracts = await this.Repository.ReadAllAsync(c => c.BuildingId == building.ID);
            foreach (var budget in activeContracts)
            {
                budget.Status = (int)ContractStatus.Finished;
                await this.Repository.UpdateAsync(budget);
            }

            // Buscar repetido
            var contract = await Repository.SingleOrDefaultAsync(c => (string.IsNullOrWhiteSpace(c.ContractNumber) ? c.ID.ToString() : c.ContractNumber).Equals(expenseVm.ContractNumber) && c.CompanyId == this.CompanyId);

            // Guardar

            if (contract != null)
            {
                var obj = await this.Repository.SingleOrDefaultAsync(contract.ID);


                obj.EditionCompleted = true;
                obj.Area = expenseVm.Area;
                obj.BuildingId = building.ID;
                obj.CustomerId = customer.ID;

                obj.ProductionRate = expenseVm.ProductionRate;
                obj.ContactSignerId = 85;
                obj.Area = expenseVm.Area;
                obj.DaysPerMonth = expenseVm.DaysPerMonth;
                obj.ExpirationDate = expenseVm.ExpirationDate;
                obj.EditionCompleted = true;
                obj.BuildingId = building.ID;
                obj.CustomerId = customer.ID;
                obj.ContractNumber = expenseVm.ContractNumber;
                obj.CompanyId = this.CompanyId;
                obj.NumberOfPeople = expenseVm.NumberOfPeople;
                obj.Status = (int)expenseVm.Status;
                obj.UnoccupiedSquareFeets = expenseVm.UnoccupiedSquareFeets;
                obj.NumberOfRestrooms = expenseVm.NumberOfRestrooms;
                obj.Description = expenseVm.Description;

                await this._RevenuesRepository.RemoveAsync(r => r.ContractId == obj.ID);

                await this._ExpensesRepository.RemoveAsync(e => e.ContractId == obj.ID);

                await this.ContractItemsRepository.RemoveAsync(i => i.ContractId == obj.ID);

                await this._ContractExpensesRepository.RemoveAsync(i => i.ContractId == obj.ID);

                await this.Repository.UpdateAsync(obj);
                Response.ErrorCode = 5;
                Response.Message = "Update Contract";
                return Response;
            }
            else
            {
                var newContract = new Contract
                {
                    ProductionRate = expenseVm.ProductionRate,
                    ContactSignerId = 85,
                    Area = expenseVm.Area,
                    DaysPerMonth = expenseVm.DaysPerMonth,
                    ExpirationDate = expenseVm.ExpirationDate,
                    EditionCompleted = true,
                    ID = 0,
                    BuildingId = building.ID,
                    CustomerId = customer.ID,
                    ContractNumber = expenseVm.ContractNumber,
                    CompanyId = this.CompanyId,
                    NumberOfPeople = expenseVm.NumberOfPeople,
                    Status = (int)expenseVm.Status,
                    UnoccupiedSquareFeets = expenseVm.UnoccupiedSquareFeets,
                    NumberOfRestrooms = expenseVm.NumberOfRestrooms,
                    Description = expenseVm.Description
                };
                await this.Repository.AddAsync(newContract);
                return Response;
            }
        }

        public async Task<ContractReportDetailViewModel> GetContractByContractNumber(string contractNumber)
        {
            return await Repository.GetContractByContractNumber(contractNumber);
        }

        #region Log
        private ContractActivityLog RegisterLogActivity(
            int contractId,
            ContractActivityType activityType,
            IList<ChangeLogEntry> changeLog = null,
            IList<ItemLogEntry> itemLog = null)
        {
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            ContractActivityLog newLog = new ContractActivityLog()
            {
                EmployeeId = employeeId,
                ContractId = contractId,
                ActivityType = activityType,
                ChangeLog = changeLog,
                ItemLog = itemLog
            };

            return this.ActivityLogRepository.Add(newLog);
        }

        private BuildingActivityLog RegisterBuildingLogActivity(
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

            return this.BuildingActivityLogRepository.Add(newLog);
        }

        private void LogContractActivity(EntityEntry entry)
        {
            int contractId = -1;

            if (entry.Entity is AuditableEntity<int> entity)
                contractId = entity.ID;

            var unwantedFields = new List<string> { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "EditionCompleted" };
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

                        if (propertyName == "Status")
                        {
                            originalValue = property.OriginalValue == null ? "" : ((ContractStatus)int.Parse(property.OriginalValue.ToString())).ToString();
                            currentValue = property.CurrentValue == null ? "" : ((ContractStatus)int.Parse(property.CurrentValue.ToString())).ToString();
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
                    contractId,
                    ContractActivityType.FieldUpdated,
                    changeLog: changeLog);
            }
        }

        private async Task RegisterContractSquareFeetUpdated(int contractId, double squareFeet, string type, int? contractItemId = null)
        {
            var currentSqftItems = await this.ContractItemsRepository.ReadAllAsync(i =>
                    i.ContractId == contractId
                    && i.RateType == ServiceRateType.SquareFeet);

            double currentSqft = currentSqftItems.Sum(i => i.SquareFeet.HasValue ? i.SquareFeet.Value : 0);
            double tempSqft = contractItemId.HasValue ? currentSqftItems.Where(i => i.ID != contractItemId.Value).Sum(i => i.SquareFeet.HasValue ? i.SquareFeet.Value : 0) : currentSqft;

            this.RegisterLogActivity(
                contractId,
                ContractActivityType.BuildingPropertyUpdated,
                new List<ChangeLogEntry>()
                {
                        new ChangeLogEntry()
                        {
                            PropertyName = $"Square Feet ({type})",
                            CurrentValue = (tempSqft + squareFeet).ToString(),
                            OriginalValue = currentSqft.ToString()
                        }
                });
        }

        private async Task RegisterContractSquareFeetRemoved(int contractId, double squareFeet)
        {
            var currentSqftItems = await this.ContractItemsRepository.ReadAllAsync(i =>
                    i.ContractId == contractId
                    && i.RateType == ServiceRateType.SquareFeet);

            double currentSqft = currentSqftItems.Sum(i => i.SquareFeet.HasValue ? i.SquareFeet.Value : 0);

            this.RegisterLogActivity(
                contractId,
                ContractActivityType.BuildingPropertyUpdated,
                new List<ChangeLogEntry>()
                {
                        new ChangeLogEntry()
                        {
                            PropertyName = "Square Feet",
                            CurrentValue = (currentSqft - squareFeet).ToString(),
                            OriginalValue = currentSqft.ToString()
                        }
                });
        }
        #endregion

        #region Log Notes
        public async Task<ContractActivityLogNote> AddActivityLogNoteAsync(ContractActivityLogNote activityLogNote)
        {
            var employeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            activityLogNote.EmployeeId = employeeId;
            return await this._ContractActivityLogNotesRepository.AddAsync(activityLogNote);
        }
        public Task<ContractActivityLogNote> UpdateActivityLogNoteAsync(ContractActivityLogNote activityLogNote)
        {
            return this._ContractActivityLogNotesRepository.UpdateAsync(activityLogNote);
        }
        public Task RemoveActivityLogNoteAsync(ContractActivityLogNote activityLogNote)
        {
            return this._ContractActivityLogNotesRepository.RemoveAsync(n => n.ID == activityLogNote.ID);
        }
        public Task<ContractActivityLogNote> GetActivityLogNoteAsync(int id)
        {
            return this._ContractActivityLogNotesRepository.SingleOrDefaultAsync(n => n.ID == id);
        }
        public Task<IEnumerable<ContractActivityLogNoteGridViewModel>> ReadAllActivityLogNotesAsync(int activityLogId)
        {
            return this._ContractActivityLogNotesRepository.ReadAllActivityLogNotesAsync(activityLogId);
        }
        #endregion

        #region Math
        private double[] CalculateBudgetRevenuesProfit(IEnumerable<ContractItem> revenues, double daysPerMonth)
        {
            var result = new double[3];
            // daily
            result[0] = 0;
            // monthly
            result[1] = 0;
            // yearly
            result[2] = 0;

            double value = 0;
            double dailyRate = 0;
            double monthlyRate = 0;
            double biMonthlyRate = 0;
            double quarterly = 0;
            double biAnnually = 0;
            double yearlyRate = 0;

            foreach (ContractItem revenue in revenues)
            {
                result[0] += revenue.DailyRate;
                result[1] += revenue.MonthlyRate;
                result[2] += revenue.YearlyRate;

                dailyRate = 0;
                monthlyRate = 0;
                biMonthlyRate = 0;
                quarterly = 0;
                biAnnually = 0;
                yearlyRate = 0;
            }

            return result;
        }

        private double[] CalculateBudgetExpensesProfit(IEnumerable<ContractExpense> expenses, double daysPerMonth)
        {
            var result = new double[3];
            // daily
            result[0] = 0;
            // monthly
            result[1] = 0;
            // yearly
            result[2] = 0;

            double value = 0;
            double rate = 0;
            double dailyRate = 0;
            double monthlyRate = 0;
            double biMonthlyRate = 0;
            double quarterly = 0;
            double biAnnually = 0;
            double yearlyRate = 0;

            double dailyLaborRate = 0;
            double monthlyLaborRate = 0;
            double yearlyLaborRate = 0;

            foreach (ContractExpense expense in expenses)
            {
                result[0] += expense.DailyRate;
                result[1] += expense.MonthlyRate;
                result[2] += expense.YearlyRate;

                if (expense.ExpenseCategory == (int)ExpenseCategory.Labor)
                {
                    dailyLaborRate += expense.DailyRate;
                    monthlyLaborRate += expense.MonthlyRate;
                    yearlyLaborRate += expense.YearlyRate;
                }

                dailyRate = 0;
                monthlyRate = 0;
                biMonthlyRate = 0;
                quarterly = 0;
                biAnnually = 0;
                yearlyRate = 0;
            }

            double totalExpensesOverheadDaily = dailyLaborRate * 0.14;
            double totalExpensesOverheadMonthly = monthlyLaborRate * 0.14;
            double totalExpensesOverheadYearly = yearlyLaborRate * 0.14;

            result[0] += totalExpensesOverheadDaily;
            result[1] += totalExpensesOverheadMonthly;
            result[2] += totalExpensesOverheadYearly;

            return result;
        }

        private double GetContractItemValueByType(ContractItem revenue)
        {
            double value = 0;
            switch (revenue.RateType)
            {
                case ServiceRateType.Hour:
                    value = revenue.Hours.HasValue ? revenue.Hours.Value : 0;
                    break;
                case ServiceRateType.Unit:
                    value = 1;
                    break;
                case ServiceRateType.Room:
                    value = revenue.Rooms.HasValue ? revenue.Rooms.Value : 0;
                    break;
                case ServiceRateType.SquareFeet:
                    value = revenue.SquareFeet.HasValue ? revenue.SquareFeet.Value : 0;
                    break;
                default:
                    break;
            }
            return value;
        }

        private double GetContractItemGridValueByType(ContractItemGridViewModel revenue)
        {
            double value = 0;
            switch (revenue.RateType)
            {
                case (int)ServiceRateType.Hour:
                    value = revenue.Hours.HasValue ? revenue.Hours.Value : 0;
                    break;
                case (int)ServiceRateType.Unit:
                    value = 1;
                    break;
                case (int)ServiceRateType.Room:
                    value = revenue.Rooms.HasValue ? revenue.Rooms.Value : 0;
                    break;
                case (int)ServiceRateType.SquareFeet:
                    value = revenue.SquareFeet.HasValue ? revenue.SquareFeet.Value : 0;
                    break;
                default:
                    break;
            }
            return value;
        }
        #endregion

        #region Notes
        public async Task<ContractNote> AddContractNoteAsync(ContractNote contractNote)
        {
            var employeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            contractNote.EmployeeId = employeeId;
            return await this._ContractNoteRepository.AddAsync(contractNote);
        }
        public Task<IEnumerable<ContractNoteGridViewModel>> ReadAllContractNotesAsync(int contractId)
        {
            return this._ContractNoteRepository.ReadAllContractNotesAsync(contractId, this.UserEmail);
        }
        #endregion
    }
}
