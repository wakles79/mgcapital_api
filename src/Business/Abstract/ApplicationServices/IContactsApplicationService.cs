// -----------------------------------------------------------------------
// <copyright file="IContactsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Contact"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Contact"/> entity
    ///     through the <see cref="IContactsRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>
    public interface IContactsApplicationService : IBaseApplicationService<Contact, int>
    {
        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="ContactPhone"/> for the current Contact        
        ///</summary>
        /// <param name="contactId"></param>
        /// <returns>A list with all the ContactPhones given ContactId</returns>
        Task<IEnumerable<ContactPhone>> ReadAllPhonesAsync(int contactId);

        /// <summary>
        ///     Adds an object to the table <see cref="ContactPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<ContactPhone> AddPhoneAsync(ContactPhone obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<ContactPhone> UpdatePhoneAsync(ContactPhone obj);

        void RemovePhone(int contactPhoneId);

        void RemovePhones(int contactId);

        Task<ContactPhone> GetPhoneByIdAsync(int contactPhoneId);

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="ContactEmail"/> for the current Contact        
        ///</summary>
        /// <param name="contactId"></param>
        /// <returns>A list with all the ContactEmails given ContactId</returns>
        Task<IEnumerable<ContactEmail>> ReadAllEmailsAsync(int contactId);

        /// <summary>
        ///     Adds an object to the table <see cref="ContactEmail"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<ContactEmail> AddEmailAsync(ContactEmail obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<ContactEmail> UpdateEmailAsync(ContactEmail obj);

        void RemoveEmail(int contactemailId);
        
        void RemoveEmails(int contactId);

        Task<ContactEmail> GetEmailByIdAsync(int contactemailId);

        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Contact"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Contact>> ReadAllAsync();

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="ContactAddress"/> for the current Contact        
        ///</summary>
        /// <param name="contactId"></param>
        /// <returns>A list with all the ContactAddress given ContactId</returns>
        Task<IEnumerable<ContactAddress>> ReadAllAddressAsync(int contactId);

        /// <summary>
        ///     Adds an object to the tables <see cref="ContactAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<ContactAddress> AddContactAddressAsync(ContactAddress obj);

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
        Task<ContactAddress> UpdateContactAddressAsync(ContactAddress obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<Address> UpdateAddressAsync(Address obj);

        void RemoveAddress(ContactAddress obj);

        void RemoveAddresses(int contactId);

        Task<ContactAddress> GetContactAddressByIdAsync(int contactId, int addressId);

        Task<CustomerContact> AddCustomerContact(CustomerContact obj);

        Task<CustomerContact> GetContactCustomerByIdAsync(int contactId, int customerId);

        void RemoveCustomerContact(CustomerContact obj);

        Task<VendorContact> AddVendorContact(VendorContact obj);

        Task<CustomerContact> UpdateCustomerContact(CustomerContact obj);

        void RemoveVendorContact(VendorContact obj);

        Task<VendorContact> UpdateVendorContact(VendorContact obj);

        Task<VendorContact> GetContactVendorByIdAsync(int contactId, int vendorId);

        Task<DataSource<ContactGridViewModel>> ReadAllCustomerContactsDapperAsync(DataSourceRequest request, int customerId);

        Task<DataSource<ContactGridViewModel>> ReadAllVendorContactsDapperAsync(DataSourceRequest request, int vendorId);

        Task<DataSource<ContactGridViewModel>> ReadAllBuildingContactsDapperAsync(DataSourceRequest request, int buildingId);

        Task<DataSource<ContactLogsViewModel>> ReadAllDapperAsync(DataSourceRequest request);
        Task<DataSource<ContactListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id);
        Task<BuildingContact> GetContactBuildingByIdAsync(int contactId, int buildingId);
        Task<Contact> GetContactByEmail(string email);
        Task<ContactEmail> GetContactEmailByContactIdAndEmail(int id, string email);
        void RemoveBuildingContact(BuildingContact contactBuildingObj);
        Task<BuildingContact> AddBuildingContactAsync(BuildingContact obj);
        Task<BuildingContact> UpdateBuildingContactAsync(BuildingContact obj);
        Task<int> PopulateEmptyCoordinatesAsync();

        Task<DataSource<ListBoxViewModel>> ReadAllBldgContactCboDapperAsync(DataSourceRequest request, int? id, int? buildingId, WorkOrderContactType type = null);
    }
}
