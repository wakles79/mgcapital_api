// -----------------------------------------------------------------------
// <copyright file="ExpensesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Expense;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ExpensesApplicationService : BaseSessionApplicationService<Expense, int>, IExpensesApplicationService
    {
        public new IExpensesRepository Repository => base.Repository as IExpensesRepository;
        private readonly IContractsRepository ContractsRepository;

        public ExpensesApplicationService(
            IExpensesRepository repository,
            IContractsRepository contractsRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor)
        {
            this.ContractsRepository = contractsRepository;
        }

        public async Task<DataSource<ExpenseGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            return await this.Repository.ReadAllDapperAsync(request, this.CompanyId, buildingId, customerId, contractId);
        }

        public async Task<IEnumerable<Expense>> AddIndirectExpenseAsync(ExpenseCreateViewModel expenseVm)
        {
            // Get the available contracts, validate if the expiration date is later or same than the date of the expense
            // and validate if the status its active

            // To Do: Add End Date to Contract to determinate this
            //var availableContracts = await this.ContractsRepository.ReadAllAsync(
            //    c => DateTime.Compare(c.ExpirationDate.Date, expenseVm.Date.Date) >= 0
            //        && c.Status == 1);
            IEnumerable<Contract> availableContracts = new List<Contract>();
            IList<Expense> addedExpenses = new List<Expense>();
            int contractsCount = 0;

            if (contractsCount == 0)
                return null;

            double amountPerBuilding = expenseVm.Amount / contractsCount;

            foreach (var contract in availableContracts)
            {
                var newExpense = new Expense()
                {
                    ID = 0,
                    ContractId = contract.ID,
                    Date = expenseVm.Date,
                    Vendor = expenseVm.Vendor,
                    Description = expenseVm.Description,
                    Type = expenseVm.Type,
                    Amount = amountPerBuilding,
                    Reference = expenseVm.Reference,
                    CompanyId = this.CompanyId,
                    TransactionNumber = expenseVm.TransactionNumber
                };
                addedExpenses.Add(await this.Repository.AddAsync(newExpense));
            }

            return addedExpenses;
        }

        public Task<DataSource<ExpenseGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request, int budgetId, int month, int year)
        {
            return this.Repository.ReadAllByBudgetIdDapperAsync(request, this.CompanyId, budgetId, month, year);
        }

        public async Task<string> AddCSVExpense(ExpenseCreateViewModel expenseVm)
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
            if (day.Length >0)
            {
                day = "0" + day;
            }
            if (month.Length > 0)
            {
                month = "0" + month;
            }
            expenseVm.TransactionNumber = month + day + year + contract.ContractNumber.ToString();

            // Buscar repetido
            var expense = await Repository.GetRepeated(contract.ID, expenseVm.Reference, expenseVm.Amount, this.CompanyId);

            //No repetido 
            if (expense.Payload.Count() > 0)
                return "2";


            // Guardar
            var newExpense = new Expense()
            {
                ID = 0,
                ContractId = contract.ID,
                Date = expenseVm.Date,
                Vendor = expenseVm.Vendor,
                Description = expenseVm.Description,
                Type = expenseVm.Type,
                Amount = expenseVm.Amount,
                Reference = expenseVm.Reference,
                CompanyId = this.CompanyId,
                TransactionNumber = expenseVm.TransactionNumber
            };

            await this.Repository.AddAsync(newExpense);
            return "0";

        }
    }
}
