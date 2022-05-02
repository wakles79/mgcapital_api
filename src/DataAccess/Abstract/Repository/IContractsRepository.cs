// -----------------------------------------------------------------------
// <copyright file="IContractsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.Expense;
using System;
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
    public interface IContractsRepository : IBaseRepository<Contract, int>
    {
        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null);

        Task<DataSource<ContractGridViewModel>> ReadAllDapperAsync(DataSourceRequestBudget request, int companyId, int? status);

        Task<DataSource<ContractExportCsvViewModel>> ReadAllCsvDapperAsync(DataSourceRequestBudget request, int companyId, int? status);

        Task<Contract> SingleOrDefaultContractByBuildingAsync(int buildingId);

        Task<ContractReportDetailViewModel> GetContractReportDetailsDapperAsync(int? contractId, Guid? guid);

        Task<DataSource<ContractListBoxViewModel>> ReadAllCboByBuildingDapperAsync(int companyId, int? buildingId, int? customerId, DateTime? date, int? contractId = null);
        Task<ContractReportDetailViewModel> GetContractReportDetailsBalancesDapperAsync(int? contractId, Guid? guid, DataSourceRequest request);

        Task<ContractTrackingDetailViewModel> GetContractTrackingDetailsDapperAsync(DataSourceRequest request, int? id, Guid? guid);

        Task<ContractSummaryViewModel> GetContractSummaryDapperAsync(int id);

        Task<ContractReportDetailViewModel> GetContractByContractNumber(string contractNumber);

        Task<ContractChildDetailViewModel> GetBudgetDetailsDapperAsync(int? id, Guid? guid, string userEmail = "");
    }
}
