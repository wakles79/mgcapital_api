using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Revenue;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class RevenuesApplicationService : BaseSessionApplicationService<Revenue, int>, IRevenuesApplicationService
    {
        private readonly IContractsRepository ContractsRepository;

        public new IRevenuesRepository Repository => base.Repository as IRevenuesRepository;

        public RevenuesApplicationService(
            IRevenuesRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IContractsRepository contractsRepository
        ) : base(repository, httpContextAccessor)
        {
            this.ContractsRepository = contractsRepository;
        }

        public Task<DataSource<RevenueGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? status = -1, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            return Repository.ReadAllDapperAsync(request, this.CompanyId, status, buildingId, customerId, contractId);
        }

        public Task<DataSource<RevenueGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request, int budgetId, int month, int year)
        {
            return this.Repository.ReadAllByBudgetIdDapperAsync(request, this.CompanyId, budgetId, month, year);
        }

        public async Task<string> AddCSVExpense(RevenueCreateViewModel expenseVm)
        {
            // Buscar contrato por el contractNumber
            var contract = await ContractsRepository.GetContractByContractNumber(expenseVm.ContractNumber);

            // No se encontro contrato
            if (contract == null)
                return "1";

            // Asignar TransationNumber 
            string day = Convert.ToString(expenseVm.Date.Day);
            string month = Convert.ToString(expenseVm.Date.Month);
            string year = Convert.ToString(expenseVm.Date.Year);
            if (day.Length > 0)
            {
                day = "0" + day;
            }
            if (month.Length > 0)
            {
                month = "0" + month;
            }
            expenseVm.TransactionNumber = month + day + year + contract.ContractNumber.ToString();

            // Buscar repetido
            var expense = await Repository.GetRepeated(contract.ID, expenseVm.Reference, expenseVm.Total, this.CompanyId);

            //No repetido 
            if (expense.Payload.Count() > 0)
                return "2";

            // Guardar
            var newRevenue = new Revenue
            {
                ID = 0,
                ContractId = contract.ID,
                Date = expenseVm.Date,
                SubTotal = expenseVm.SubTotal,
                Tax = expenseVm.Tax,
                Total = expenseVm.Total,
                Description = expenseVm.Description,
                Reference = expenseVm.Reference,
                TransactionNumber = expenseVm.TransactionNumber,
                CompanyId = this.CompanyId
            };

            await this.Repository.AddAsync(newRevenue);
            return "0";

        }
    }
}
