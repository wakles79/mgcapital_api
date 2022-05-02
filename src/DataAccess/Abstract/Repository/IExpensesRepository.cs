// -----------------------------------------------------------------------
// <copyright file="IExpenseRepository.cs" company="Axzes">
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
    public interface IExpensesRepository : IBaseRepository<Expense, int>
    {
        Task<DataSource<ExpenseGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? buildingId = null, int? customerId = null, int? contractId = null);

        Task<DataSource<ExpenseGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request,int companyId, int budgetId, int month, int year);

        Task<DataSource<ExpenseGridViewModel>> GetRepeated(int contractId, string reference, double amount, int companyId);
    }
}
