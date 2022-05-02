// -----------------------------------------------------------------------
// <copyright file="ICustomersRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Customer;
using MGCap.Domain.ViewModels.DataViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface ICustomersRepository : IBaseRepository<Customer, int>
    {
        Task<DataSource<CustomerGridViewModel>> ReadAllAsyncDapper(DataSourceRequestCustomer request, int companyId);

        Task<DataSource<CustomerListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestCustomer request, int companyId, int? id);

        Task<Customer> GetCustomerByContractNumber(string customerCode);
    }
}
