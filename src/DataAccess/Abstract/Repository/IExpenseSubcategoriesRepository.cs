// -----------------------------------------------------------------------
// <copyright file="IExpenseSubcategoriesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="ExpenseSubcategory"/>
    /// </summary>
    public interface IExpenseSubcategoriesRepository : IBaseRepository<ExpenseSubcategory, int>
    {
        Task<DataSource<ExpenseSubcategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request,int expenseTypeId, int? isEnabled);

        Task<IEnumerable<ExpenseSubcategory>> FindByName(int companyId, string name);
    }
}
