// -----------------------------------------------------------------------
// <copyright file="ICustomersApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Customer;
using MGCap.Domain.ViewModels.CustomerEmployee;
using MGCap.Domain.ViewModels.DataViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Customer"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Customer"/> entity
    ///     through the <see cref="ICustomersRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>
    public interface ICustomersApplicationService : IBaseApplicationService<Customer,int>
    {
        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Customer"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IQueryable<Customer>> ReadAllAsync();

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="CustomerPhone"/> for the current Customer        
        ///</summary>
        /// <param name="customerId"></param>
        /// <returns>A list with all the CustomerPhones given customerId</returns>
        Task<IEnumerable<CustomerPhone>> ReadAllPhonesAsync(int customerId);

        /// <summary>
        ///     Adds an object to the table <see cref="CustomerPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<CustomerPhone> AddPhoneAsync(CustomerPhone obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<CustomerPhone> UpdatePhoneAsync(CustomerPhone obj);

        void RemovePhone(int customerPhoneId);

        Task<CustomerPhone> GetPhoneByIdAsync(int customerPhoneId);

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="CustomerAddress"/> for the current Customer        
        ///</summary>
        /// <param name="customerId"></param>
        /// <returns>A list with all the CustomerAddress given ContactId</returns>
        Task<IEnumerable<CustomerAddress>> ReadAllAddressAsync(int customerId);

        /// <summary>
        ///     Adds an object to the tables <see cref="CustomerAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<CustomerAddress> AddCustomerAddressAsync(CustomerAddress obj);

        /// <summary>
        ///     Adds an object to the tables <see cref="Address"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<Address> AddAddressAsync(Address obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<CustomerAddress> UpdateCustomerAddressAsync(CustomerAddress obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<Address> UpdateAddressAsync(Address obj);

        void RemoveAddress(CustomerAddress obj);

        Task<CustomerAddress> GetContactAddressByIdAsync(int customerId, int addressId);

        Task<DataSource<CustomerGridViewModel>> ReadAllDapperAsync(DataSourceRequestCustomer request);

        Task<IEnumerable<CustomerCsvViewModel>> ReadAllToCsv(DataSourceRequestCustomer request);

        Task<IEnumerable<CustomerCustomerGroup>> ReadAllCustomerGroupAsync(int? customerId);

        Task<IEnumerable<CustomerGroup>> ReadAllCustomerGroupAsync();

        void RemoveAllCustomerCustomerGroups(int customerId);

        void AssignCustomerGroups(List<int> groupsId, int customerId);

        Task<DataSource<CustomerEmployeeGridViewModel>> ReadAllEmployeesAsyncDapper(DataSourceRequest request, int customerId);

        Task<DataSource<CustomerListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestCustomer request, int? id);

        Task<IQueryable<CustomerContact>> ReadAllCustomerContactsAsync(int customerId);
    } 
}
