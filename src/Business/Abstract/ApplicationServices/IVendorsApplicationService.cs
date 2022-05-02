// -----------------------------------------------------------------------
// <copyright file="IVendorsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Vendor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Vendor"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Vendor"/> entity
    ///     through the <see cref="IContactsRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>
    public interface IVendorsApplicationService : IBaseApplicationService<Vendor, int>
    {
        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Vendor"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Vendor>> ReadAllAsync();

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="VendorPhone"/> for the current Vendor        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the VendorPhones given vendorId</returns>
        Task<IEnumerable<VendorPhone>> ReadAllPhonesAsync(int vendorId);

        /// <summary>
        ///     Adds an object to the table <see cref="VendorPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<VendorPhone> AddPhoneAsync(VendorPhone obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<VendorPhone> UpdatePhoneAsync(VendorPhone obj);

        void RemovePhone(int vendorPhoneId);

        Task<VendorPhone> GetPhoneByIdAsync(int vendorPhoneId);

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="VendorEmail"/> for the current Vendor        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the VendorEmails given vendorId</returns>
        Task<IEnumerable<VendorEmail>> ReadAllEmailsAsync(int vendorId);

        /// <summary>
        ///     Adds an object to the table <see cref="VendorEmail"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<VendorEmail> AddEmailAsync(VendorEmail obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<VendorEmail> UpdateEmailAsync(VendorEmail obj);

        void RemoveEmail(int vendorEmailId);

        Task<VendorEmail> GetEmailByIdAsync(int vendorEmailId);

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="VendorAddress"/> for the current Contact        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the ContactAddress given ContactId</returns>
        Task<IEnumerable<VendorAddress>> ReadAllAddressAsync(int vendorId);

        /// <summary>
        ///     Adds an object to the tables <see cref="VendorAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<VendorAddress> AddContactAddressAsync(VendorAddress obj);

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
        Task<VendorAddress> UpdateContactAddressAsync(VendorAddress obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<Address> UpdateAddressAsync(Address obj);

        void RemoveAddress(VendorAddress obj);

        Task<VendorAddress> GetVendorAddressByIdAsync(int vendorId, int addressId);

        Task<DataSource<VendorGridViewModel>> ReadAllDapperAsync(DataSourceRequest request);

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="VendorContact"/> for the current Vendor        
        ///</summary>
        /// <param name="vendorId"></param>
        /// <returns>A list with all the VendorContacts given VendorId</returns>
        Task<IEnumerable<VendorContact>> ReadAllContactsAsync(int vendorId);

        /// <summary>
        ///     Adds an object to the tables <see cref="VendorContact"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<VendorContact> AddVendorContactAsync(VendorContact obj);

        void RemoveContact(VendorContact obj);

        Task<VendorContact> GetVendorContactByIdAsync(int vendorId, int contactId);
        Task<IEnumerable<VendorVendorGroup>> ReadAllVendorGroupAsync(int? vendorId);

        Task<IEnumerable<VendorGroup>> ReadAllVendorGroupAsync();

        void RemoveAllVendorVendorGroups(int vendorId);

        void AssignVendorGroups(List<int> groupsId, int vendorId);

        Task<DataSource<ListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int? id = null);

    }
}
