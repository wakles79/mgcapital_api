// -----------------------------------------------------------------------
// <copyright file="IExpensesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IExpensesApplicationService : IBaseApplicationService<Expense, int>
    {
        Task<DataSource<ExpenseGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? buildingId = null, int? customerId = null, int? contractId = null);

        Task<DataSource<ExpenseGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request, int budgetId, int month, int year);

        Task<IEnumerable<Expense>> AddIndirectExpenseAsync(ExpenseCreateViewModel expenseVm);

        Task<string> AddCSVExpense(ExpenseCreateViewModel expenseVm);
    }
}
