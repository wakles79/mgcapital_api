// -----------------------------------------------------------------------
// <copyright file="CustomersApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CustomerEmployee;
using MGCap.Domain.ViewModels.DataViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Customer;

namespace MGCap.Business.Implementation.ApplicationServices
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
    public class CustomersApplicationService : BaseSessionApplicationService<Customer, int>, ICustomersApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomersApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="CustomersRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="customerPhonesRepository">
        ///     To inject the implementation of <see cref="CustomerPhonesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="customerAddressesRepository">
        ///     To inject the implementation of <see cref="CustomerAddressesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param> 
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>

        public CustomersApplicationService(
            ICustomersRepository repository,
            IAddressesRepository addressesRepository,
            ICustomerPhonesRepository customerPhonesRepository,
            ICustomerAddressesRepository customerAddressesRepository,
            ICustomerGroupsRepository customerGroupsRepository,
            ICustomerCustomerGroupsRepository customerCustomerGroupsRepository,
            ICustomerEmployeesRepository customerEmployeesRepository,
            ICustomerContactsRepository customerContactsRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            AddressesRepository = addressesRepository;
            CustomerPhonesRepository = customerPhonesRepository;
            CustomerAddressesRepository = customerAddressesRepository;
            CustomerCustomerGroupsRepository = customerCustomerGroupsRepository;
            CustomerGroupsRepository = customerGroupsRepository;
            CustomerEmployeesRepository = customerEmployeesRepository;
            CustomerContactsRepository = customerContactsRepository;
        }

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new ICustomersRepository Repository => base.Repository as ICustomersRepository;

        /// <summary>
        /// AddressesRepository DI
        /// </summary>
        private readonly IAddressesRepository AddressesRepository;

        /// <summary>
        /// CustomerPhonesRepository DI
        /// </summary>
        private readonly ICustomerPhonesRepository CustomerPhonesRepository;

        /// <summary>
        /// CustomerAddressesRepository DI
        /// </summary>
        private readonly ICustomerAddressesRepository CustomerAddressesRepository;

        /// <summary>
        /// CustomerGroupsRepository DI
        /// </summary>
        private readonly ICustomerGroupsRepository CustomerGroupsRepository;

        /// <summary>
        /// CustumerCustomerGroupsRepository DI
        /// </summary>
        private readonly ICustomerCustomerGroupsRepository CustomerCustomerGroupsRepository;

        private readonly ICustomerEmployeesRepository CustomerEmployeesRepository;

        private readonly ICustomerContactsRepository CustomerContactsRepository;

        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Customer"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IQueryable<Customer>> ReadAllAsync()
        {
            return await Repository.ReadAllAsync(CompanyFilter);
        }

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="CustomerPhone"/> for the current Contact        
        ///</summary>
        /// <param name="customerId"></param>
        /// <returns>A list with all the ContactPhones given ContactId</returns>
        public async Task<IEnumerable<CustomerPhone>> ReadAllPhonesAsync(int customerId)
        {
            var phones = await CustomerPhonesRepository.ReadAllAsync(
                                entity => entity.CustomerId == customerId);

            return phones;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="CustomerPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<CustomerPhone> AddPhoneAsync(CustomerPhone obj)
        {
            return await CustomerPhonesRepository.AddAsync(obj);
        }

        public async Task<CustomerPhone> UpdatePhoneAsync(CustomerPhone obj)
        {
            return await CustomerPhonesRepository.UpdateAsync(obj);
        }

        public void RemovePhone(int customerPhoneId)
        {
            CustomerPhonesRepository.Remove(ent => ent.ID == customerPhoneId);
        }

        public Task<CustomerPhone> GetPhoneByIdAsync(int customerPhoneId)
        {
            return CustomerPhonesRepository.SingleOrDefaultAsync(ent => ent.ID == customerPhoneId);
        }

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="CustomerAddress"/> for the current Customer        
        ///</summary>
        /// <param name="customerId"></param>
        /// <returns>A list with all the ContactAddress given ContactId</returns>
        public async Task<IEnumerable<CustomerAddress>> ReadAllAddressAsync(int customerId)
        {
            var address = await CustomerAddressesRepository.ReadAllAsync(
                                entity => entity.CustomerId == customerId);

            return address;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="CustomerAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<CustomerAddress> AddCustomerAddressAsync(CustomerAddress obj)
        {
            return await CustomerAddressesRepository.AddAsync(obj);
        }

        public async Task<CustomerAddress> UpdateCustomerAddressAsync(CustomerAddress obj)
        {
            return await CustomerAddressesRepository.UpdateAsync(obj);
        }

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public async Task<Address> UpdateAddressAsync(Address obj)
        {
            return await AddressesRepository.UpdateAsync(obj);
        }

        public async Task<Address> AddAddressAsync(Address obj)
        {
            return await AddressesRepository.AddAsync(obj);
        }

        public void RemoveAddress(CustomerAddress obj)
        {
            CustomerAddressesRepository.Remove(obj);
        }

        public Task<CustomerAddress> GetContactAddressByIdAsync(int customerId, int addressId)
        {
            return CustomerAddressesRepository.SingleOrDefaultAsync(ent => ent.CustomerId == customerId && ent.AddressId == addressId);
        }

        public async Task<DataSource<CustomerGridViewModel>> ReadAllDapperAsync(DataSourceRequestCustomer request)
        {
            return await Repository.ReadAllAsyncDapper(request, this.CompanyId);
        }

        public async Task<IEnumerable<CustomerCsvViewModel>> ReadAllToCsv(DataSourceRequestCustomer request)
        {
            var customers = await Repository.ReadAllAsyncDapper(request, this.CompanyId);

            IEnumerable<CustomerCsvViewModel> result = customers.Payload
                .Select(c => new CustomerCsvViewModel()
                {
                    CustomerCode = c.Code,
                    Name = c.Name,
                    Phone = c.Phone,
                    Address = c.FullAddress,
                    ContactsTotal = c.ContactsTotal,
                }).ToList();
            return result;
        }

        public async Task<IEnumerable<CustomerCustomerGroup>> ReadAllCustomerGroupAsync(int? customerId)
        {
            return await CustomerCustomerGroupsRepository.ReadAllAsync(ent => ent.CustomerId == customerId);
        }

        public async Task<IEnumerable<CustomerGroup>> ReadAllCustomerGroupAsync()
        {
            return await CustomerGroupsRepository.ReadAllAsync(ent => ent.CompanyId == CompanyId);
        }

        public void RemoveAllCustomerCustomerGroups(int customerId)
        {
            CustomerCustomerGroupsRepository.Remove(ent => ent.CustomerId == customerId);
        }

        public void AssignCustomerGroups(List<int> groupsId, int customerId)
        {
            foreach (var id in groupsId)
            {
                CustomerCustomerGroupsRepository.AddAsync(new CustomerCustomerGroup { CustomerId = customerId, CustomerGroupId = id });
            }
        }

        public async Task<DataSource<CustomerEmployeeGridViewModel>> ReadAllEmployeesAsyncDapper(DataSourceRequest request, int customerId)
        {
            return await CustomerEmployeesRepository.ReadAllAsyncDapper(request, customerId);
        }

        public Task<DataSource<CustomerListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestCustomer request, int? id)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId, id);
        }

        public async Task<IQueryable<CustomerContact>> ReadAllCustomerContactsAsync(int customerId)
        {
            return await CustomerContactsRepository.ReadAllAsync(ent => ent.CustomerId == customerId);
        }
    }
}
