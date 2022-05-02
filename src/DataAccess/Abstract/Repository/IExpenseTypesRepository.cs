// -----------------------------------------------------------------------
// <copyright file="IExpenseTypesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseType;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="ExpenseType"/>
    /// </summary>
    public interface IExpenseTypesRepository : IBaseRepository<ExpenseType, int>
    {
        Task<DataSource<ExpenseTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1);

        Task<DataSource<ExpenseTypeListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId);
    }
}
