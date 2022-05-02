// -----------------------------------------------------------------------
// <copyright file="VendorsApplicationService.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Vendor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Vendors"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Vendors"/> entity
    ///     through the <see cref="IVendorsRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>   
    public class VendorsApplicationService : BaseSessionApplicationService<Vendor, int>, IVendorsApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VendorsApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="VendorsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="vendorPhonesRepository">
        ///     To inject the implementation of <see cref="VendorPhonesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>   
        /// <param name="vendorAddressesRepository">
        ///     To inject the implementation of <see cref="VendorAddressesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param> 
        /// <param name="vendorContactsRepository">
        ///     To inject the implementation of <see cref="VendorContactsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public VendorsApplicationService(
            IVendorsRepository repository,
            IVendorPhonesRepository vendorPhonesRepository,
            IVendorEmailsRepository vendorEmailsRepository,
            IVendorAddressesRepository vendorAddressesRepository,
            IAddressesRepository addressesRepository,
            IVendorContactsRepository vendorContactsRepository,
            IVendorGroupsRepository vendorGroupsRepository,
            IVendorVendorGroupsRepository vendorVendorGroupsRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            VendorPhonesRepository = vendorPhonesRepository;
            VendorEmailsRepository = vendorEmailsRepository;
            VendorAddressesRepository = vendorAddressesRepository;
            AddressesRepository = addressesRepository;
            VendorContactsRepository = vendorContactsRepository;
            VendorGroupsRepository = vendorGroupsRepository;
            VendorVendorGroupsRepository = vendorVendorGroupsRepository;
        }

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new IVendorsRepository Repository => base.Repository as IVendorsRepository;

        /// <summary>
        /// VendorPhonesRepository DI
        /// </summary>
        private readonly IVendorPhonesRepository VendorPhonesRepository;

        /// <summary>
        /// VendorEmailsRepository DI
        /// </summary>
        private readonly IVendorEmailsRepository VendorEmailsRepository;

        /// <summary>
        /// VendorAddressesRepository DI
        /// </summary>
        private readonly IVendorAddressesRepository VendorAddressesRepository;

        /// <summary>
        /// AddressesRepository DI
        /// </summary>
        private readonly IAddressesRepository AddressesRepository;

        /// <summary>
        /// AddressesRepository DI
        /// </summary>
        private readonly IVendorContactsRepository VendorContactsRepository;

        /// <summary>
        /// VendorGroupsRepository DI
        /// </summary>
        private readonly IVendorGroupsRepository VendorGroupsRepository;

        /// <summary>
        /// VendorVendorGroupsRepository DI
        /// </summary>
        private readonly IVendorVendorGroupsRepository VendorVendorGroupsRepository;

        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Vendor"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Vendor>> ReadAllAsync()
        {
            return await Repository.ReadAllAsync(CompanyFilter);
        }

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="VendorPhone"/> for the current Contact        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the ContactPhones given ContactId</returns>
        public async Task<IEnumerable<VendorPhone>> ReadAllPhonesAsync(int vendorId)
        {
            var phones = await VendorPhonesRepository.ReadAllAsync(
                                entity => entity.VendorId == vendorId);

            return phones;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="VendorPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<VendorPhone> AddPhoneAsync(VendorPhone obj)
        {
            return await VendorPhonesRepository.AddAsync(obj);
        }

        public async Task<VendorPhone> UpdatePhoneAsync(VendorPhone obj)
        {
            return await VendorPhonesRepository.UpdateAsync(obj);
        }

        public void RemovePhone(int vendorPhoneId)
        {
            VendorPhonesRepository.Remove(ent => ent.ID == vendorPhoneId);
        }

        public Task<VendorPhone> GetPhoneByIdAsync(int vendorPhoneId)
        {
            return VendorPhonesRepository.SingleOrDefaultAsync(ent => ent.ID == vendorPhoneId);
        }
        public async Task<IEnumerable<VendorEmail>> ReadAllEmailsAsync(int vendorId)
        {
            var emails = await VendorEmailsRepository.ReadAllAsync(
                    entity => entity.VendorId == vendorId);

            return emails;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="VendorEmail"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<VendorEmail> AddEmailAsync(VendorEmail obj)
        {
            return await VendorEmailsRepository.AddAsync(obj);
        }

        public async Task<VendorEmail> UpdateEmailAsync(VendorEmail obj)
        {
            return await VendorEmailsRepository.UpdateAsync(obj);
        }

        public void RemoveEmail(int contactEmailId)
        {
            VendorEmailsRepository.Remove(ent => ent.ID == contactEmailId);
        }

        public Task<VendorEmail> GetEmailByIdAsync(int vendorEmailId)
        {
            return VendorEmailsRepository.SingleOrDefaultAsync(ent => ent.ID == vendorEmailId);
        }

        /// <summary>
        /// Asynchronously read all the VendorAddress"/> for the current Contact        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the ContactAddress given ContactId</returns>
        public async Task<IEnumerable<VendorAddress>> ReadAllAddressAsync(int vendorId)
        {
            var address = await VendorAddressesRepository.ReadAllAsync(
                                entity => entity.VendorId == vendorId);

            return address;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="VendorAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<VendorAddress> AddContactAddressAsync(VendorAddress obj)
        {
            return await VendorAddressesRepository.AddAsync(obj);
        }

        public async Task<VendorAddress> UpdateContactAddressAsync(VendorAddress obj)
        {
            return await VendorAddressesRepository.UpdateAsync(obj);
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

        public void RemoveAddress(VendorAddress obj)
        {
            VendorAddressesRepository.Remove(obj);
        }

        public Task<VendorAddress> GetVendorAddressByIdAsync(int vendorId, int addressId)
        {
            return VendorAddressesRepository.SingleOrDefaultAsync(ent => ent.VendorId == vendorId && ent.AddressId == addressId);
        }


        public async Task<DataSource<VendorGridViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return await Repository.ReadAllAsyncDapper(request, this.CompanyId);
        }

        public async Task<IEnumerable<VendorContact>> ReadAllContactsAsync(int vendorId)
        {
            var contacts = await VendorContactsRepository.ReadAllAsync(
                    entity => entity.VendorId == vendorId);

            return contacts;
        }

        public async Task<VendorContact> AddVendorContacAsync(VendorContact obj)
        {
            return await VendorContactsRepository.AddAsync(obj);
        }

        public async Task<VendorContact> AddVendorContactAsync(VendorContact obj)
        {
            return await VendorContactsRepository.AddAsync(obj);
        }

        public void RemoveContact(VendorContact obj)
        {
            VendorContactsRepository.Remove(obj);
        }

        public Task<VendorContact> GetVendorContactByIdAsync(int vendorId, int contactId)
        {
            return VendorContactsRepository.SingleOrDefaultAsync(ent => ent.VendorId == vendorId && ent.ContactId == contactId);
        }

        public async Task<IEnumerable<VendorVendorGroup>> ReadAllVendorGroupAsync(int? vendorId)
        {
            return await VendorVendorGroupsRepository.ReadAllAsync(ent => ent.VendorId == vendorId);
        }

        public async Task<IEnumerable<VendorGroup>> ReadAllVendorGroupAsync()
        {
            return await VendorGroupsRepository.ReadAllAsync(ent => ent.CompanyId == CompanyId);
        }

        public void RemoveAllVendorVendorGroups(int vendorId)
        {
            VendorVendorGroupsRepository.Remove(ent => ent.VendorId == vendorId);
        }

        public void AssignVendorGroups(List<int> groupsId, int vendorId)
        {
            foreach (var id in groupsId)
            {
                VendorVendorGroupsRepository.AddAsync(new VendorVendorGroup { VendorId = vendorId, VendorGroupId = id });
            }
        }

        public Task<DataSource<ListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int? id = null)
        {
            return Repository.ReadAllCboAsyncDapper(request, CompanyId, id);
        }
    }
}
