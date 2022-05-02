// -----------------------------------------------------------------------
// <copyright file="IContractsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractExpense;
using System;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using MGCap.Domain.ViewModels.ContractActivityLog;
using MGCap.Domain.ViewModels.ContractNote;
using MGCap.Domain.ViewModels.ContractActivityLogNote;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IContractsApplicationService : IBaseApplicationService<Contract, int>
    {
        Task<Contract> AddContractAsync(Contract contract);
        /// <summary>
        ///     Reads all elements in a list-like form
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null);
        Task<DataSource<ContractGridViewModel>> ReadAllDapperAsync(DataSourceRequestBudget request, int? status);
        Task<IEnumerable<ContractExportCsvViewModel>> ReadAllToCsvAsync(DataSourceRequestBudget request, int? status);
        Task<DataSource<ContractListBoxViewModel>> ReadAllCboByBuildingDapperAsync(int? buildingId, int? customerId, DateTime? date, int? contractId = null);
        Task<Contract> SingleOrDefaultContractByBuildingAsync(int buildingId);
        Task<bool> VerifyContractNumberExists(string contractNumber, int contractId = -1);
        Task DeleteContractAsync(int id);
        Task DisableBuildingContracts(int buildingId);
        Task AddDefaultItemsAndExpensesToContract(int contractId);
        Task UpdateDefaultItemsAndExpensesToContract(int contractId);
        Task<DataSource<ContractActivityLogGridViewModel>> ReadAllActivityLog(DataSourceRequest request, int contractId);
        Task UpdateBudgetProfit(int budgetId);
        Task<string> GetBudgetDocumentUrl(int budgetId);
        Task<ContractChildDetailViewModel> GetBudgetDetail(int? id, Guid? guid);
        Task UpdateContractChildrenPeriodicityRate(int contractId, double daysPerMonth);
        Task<ContractSummaryViewModel> GetContractSummaryAsync(int id);

        Task<ContractItem> AddContractItemAsync(ContractItem contractItem);
        Task<ContractItem> UpdateContractItemAsync(ContractItem contractItem);
        Task<ContractItem> GetContractItemByIdAsync(int id);
        Task<DataSource<ContractItemGridViewModel>> ReadAllContractItemsDapperAsync(DataSourceRequest request, int contractId);
        Task RemoveContractItemAsync(ContractItem contractItem);
        Task<ContractHttpCsvResponseViewModel> AddContractItemFromCsvAsync(ContractItemCsvViewModel vm);
        Task<IEnumerable<ContractOfficeSpaceGridViewModel>> ReadAllRevenuesSquareFeetAsync(int contractId);
        Task UpdateContractItemOrder(ContractItemUpdateOrderViewModel vm);

        Task<ContractExpense> AddContractExpenseAsync(ContractExpense contractExpense);
        Task<ContractExpense> UpdateContractExpenseAsync(ContractExpense contractExpense);
        Task<ContractExpense> GetContractExpenseByIdAsync(int id);
        Task<DataSource<ContractExpenseGridViewModel>> ReadAllContractExpensesDapperAsync(DataSourceRequest request, int contractId);
        Task RemoveContractExpenseAsync(ContractExpense contractExpense);
        Task<ContractHttpCsvResponseViewModel> AddContractExpenseFromCsv(ContractExpenseCsvViewModel vm);

        Task<DataSource<ContractOfficeSpaceGridViewModel>> ReadAllOfficeSpacesDapperAsync(DataSourceRequest request, int contractId);
        Task<ContractOfficeSpace> AddOfficeSpaceAsync(ContractOfficeSpace officeSpace);
        Task<ContractOfficeSpace> GetOfficeSpaceByIdAsync(int officeSpaceId);
        Task<ContractOfficeSpace> UpdateOfficeSpaceAsync(ContractOfficeSpace officeSpace);
        Task<ContractReportDetailViewModel> GetContractReportDetailsDapperAsync(int? contractId, Guid? guid);

        Task<ContractReportDetailViewModel> GetContractReportDetailsBalancesDapperAsync(int id, Guid? contractId, DataSourceRequest request);

        Task<ContractTrackingDetailViewModel> GetContractTrackingDetailsDapperAsync(DataSourceRequest request, int? id, Guid? guid);

        Task<ContractHttpCsvResponseViewModel> AddContractFromCsvAsync(ContractCreateViewModel contractVm);

        Task<ContractReportDetailViewModel> GetContractByContractNumber(string contractNumber);


        #region Notes
        Task<ContractNote> AddContractNoteAsync(ContractNote contractNote);
        Task<IEnumerable<ContractNoteGridViewModel>> ReadAllContractNotesAsync(int contractId);
        #endregion

        #region Log Notes
        Task<ContractActivityLogNote> AddActivityLogNoteAsync(ContractActivityLogNote activityLogNote);
        Task<ContractActivityLogNote> UpdateActivityLogNoteAsync(ContractActivityLogNote activityLogNote);
        Task RemoveActivityLogNoteAsync(ContractActivityLogNote activityLogNote);
        Task<ContractActivityLogNote> GetActivityLogNoteAsync(int id);
        Task<IEnumerable<ContractActivityLogNoteGridViewModel>> ReadAllActivityLogNotesAsync(int activityLogId);
        #endregion
    }
}
