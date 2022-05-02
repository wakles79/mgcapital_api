// -----------------------------------------------------------------------
// <copyright file="IExpenseTypesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using MGCap.Domain.ViewModels.ExpenseType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IExpenseTypesApplicationService : IBaseApplicationService<ExpenseType, int>
    {
        Task<DataSource<ExpenseTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = -1);

        Task<DataSource<ExpenseTypeListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request);

        Task<ExpenseType> UpdateStatusAsync(int id);

        Task<ExpenseSubcategory> AddSubcategoryAsync(ExpenseSubcategory expenseSubcategory);
        Task<ExpenseSubcategory> UpdateSubcategoryAsync(ExpenseSubcategory expenseSubcategory);
        Task<ExpenseSubcategory> GetSubcategoryAsync(int id);
        Task<DataSource<ExpenseSubcategoryListBoxViewModel>> ReadAllSubcategoriesCboDapperAsync(DataSourceRequest request, int expenseTypeId, int? isEnabled);
    }
}
