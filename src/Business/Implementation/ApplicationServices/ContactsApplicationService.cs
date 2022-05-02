// -----------------------------------------------------------------------
// <copyright file="ContactsApplicationService.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Geocoding;
using MGCap.Domain.Options;
using Microsoft.Extensions.Options;
using Geocoding.Google;
using System.Linq;
using MGCap.DataAccess.Implementation.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Business.Implementation.ApplicationServices
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
    public class ContactsApplicationService : BaseSessionApplicationService<Contact, int>, IContactsApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactsApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="ContactsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="contactPhonesRepository">
        ///     To inject the implementation of <see cref="ContactPhonesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="contactEmailsRepository">
        ///     To inject the implementation of <see cref="ContactEmailsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="ContactAddressesRepository">
        ///     To inject the implementation of <see cref="ContactAddressesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param> 
        /// <param name="customerContactsRepository">
        ///     To inject the implementation of <see cref="CustomerContactsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param> 
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public ContactsApplicationService(
            IContactsRepository repository,
            IContactPhonesRepository contactPhonesRepository,
            IContactEmailsRepository contactEmailsRepository,
            IContactAddressesRepository contactAddressesRepository,
            IAddressesRepository addressesRepository,
            ICustomerContactsRepository customerContactsRepository,
            IVendorContactsRepository vendorContactsRepository,
            IBuildingContactsRepository buildingContactsRepository,
            IHttpContextAccessor httpContextAccessor,
            IOptions<GeocoderOptions> geocoderOptions)
            : base(repository, httpContextAccessor)
        {
            ContactPhonesRepository = contactPhonesRepository;
            ContactEmailsRepository = contactEmailsRepository;
            ContactAddressesRepository = contactAddressesRepository;
            CustomerContactsRepository = customerContactsRepository;
            VendorContactsRepository = vendorContactsRepository;
            BuildingContactsRepository = buildingContactsRepository;
            AddressesRepository = addressesRepository;

            this.Geocoder = new GoogleGeocoder
            {
                ApiKey = geocoderOptions.Value.GoogleGeocoderApiKey
            };

        }

        /// <summary>
        ///     Gets a google geocoder object
        /// </summary>
        public readonly IGeocoder Geocoder;

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new IContactsRepository Repository => base.Repository as IContactsRepository;

        /// <summary>
        /// ContactPhonesRepository DI
        /// </summary>
        private readonly IContactPhonesRepository ContactPhonesRepository;

        /// <summary>
        /// ContactEmailsRepository DI
        /// </summary>
        private readonly IContactEmailsRepository ContactEmailsRepository;

        /// <summary>
        /// ContactAddressesRepository DI
        /// </summary>
        private readonly IContactAddressesRepository ContactAddressesRepository;

        /// <summary>
        /// AddressesRepository DI
        /// </summary>
        private readonly IAddressesRepository AddressesRepository;

        /// <summary>
        /// CustomerContactRepository DI
        /// </summary>
        private readonly ICustomerContactsRepository CustomerContactsRepository;

        /// <summary>
        /// VendorContactsRepository DI
        /// </summary>
        private readonly IVendorContactsRepository VendorContactsRepository;

        /// <summary>
        ///     BuildingContactsRepository DI
        /// </summary>
        private readonly IBuildingContactsRepository BuildingContactsRepository;

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="ContactPhone"/> for the current Contact        
        ///</summary>
        /// <param name="contactId"></param>
        /// <returns>A list with all the ContactPhones given ContactId</returns>
        public async Task<IEnumerable<ContactPhone>> ReadAllPhonesAsync(int contactId)
        {
            var phones = await ContactPhonesRepository.ReadAllAsync(
                                entity => entity.ContactId == contactId);

            return phones;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="ContactPhone"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<ContactPhone> AddPhoneAsync(ContactPhone obj)
        {
            return await ContactPhonesRepository.AddAsync(obj);
        }

        public async Task<ContactPhone> UpdatePhoneAsync(ContactPhone obj)
        {
            return await ContactPhonesRepository.UpdateAsync(obj);
        }

        public void RemovePhone(int contactPhoneId)
        {
            ContactPhonesRepository.Remove(ent => ent.ID == contactPhoneId);
        }

        public void RemovePhones(int contactId)
        {
            ContactPhonesRepository.Remove(ent => ent.ContactId == contactId);
        }

        public Task<ContactPhone> GetPhoneByIdAsync(int contactPhoneId)
        {
            return ContactPhonesRepository.SingleOrDefaultAsync(ent => ent.ID == contactPhoneId);
        }

        public async Task<IEnumerable<ContactEmail>> ReadAllEmailsAsync(int contactId)
        {
            var emails = await ContactEmailsRepository.ReadAllAsync(
                    entity => entity.ContactId == contactId);

            return emails;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="ContactEmail"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<ContactEmail> AddEmailAsync(ContactEmail obj)
        {
            //bool exists = await ContactEmailsRepository.ExistsAsync(ce => ce.Email.Equals(obj.Email) && ce.Default && ce.Contact.CompanyId.Equals(this.CompanyId));
            var contactEmail = await ContactEmailsRepository.Entities
                                                            .Include(ce => ce.Contact)
                                                            .FirstOrDefaultAsync(ce => ce.Email.Equals(obj.Email) && ce.Default);

            if(contactEmail != null && contactEmail.Contact.CompanyId.Equals(this.CompanyId))
            {
                throw new DuplicateNameException("There is already a contact with this email as its default.", null);
            }

            return await ContactEmailsRepository.AddAsync(obj);
        }

        public async Task<ContactEmail> UpdateEmailAsync(ContactEmail obj)
        {
            return await ContactEmailsRepository.UpdateAsync(obj);
        }

        public void RemoveEmail(int contactEmailId)
        {
            ContactEmailsRepository.Remove(ent => ent.ID == contactEmailId);
        }

        public void RemoveEmails(int contactId)
        {
            ContactEmailsRepository.Remove(c => c.ContactId == contactId);
        }

        public Task<ContactEmail> GetEmailByIdAsync(int contactEmailId)
        {
            return ContactEmailsRepository.SingleOrDefaultAsync(ent => ent.ID == contactEmailId);
        }

        public async Task<Contact> GetContactByEmail(string email)
        {
            ContactEmail contactEmail= await ContactEmailsRepository.FirstOrDefaultAsync(entity => entity.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
            if(contactEmail != null)
            {
                return await Repository.SingleOrDefaultAsync(entity => entity.ID == contactEmail.ContactId);
            }
            return null;
        }

        /// <summary>
        /// Returns a ContactEmail based on a contactId and email
        /// </summary>
        /// <param name="contactId">Contact ID</param>
        /// <param name="email">email</param>
        /// <returns>ContactEmail</returns>
        public async Task<ContactEmail> GetContactEmailByContactIdAndEmail(int contactId,string email)
        {
            ContactEmail contactEmail = await ContactEmailsRepository.FirstOrDefaultAsync(entity => entity.ContactId == contactId && entity.Email == email);
            if (contactEmail != null) return contactEmail;
            return null;
        }

        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Employee"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Contact>> ReadAllAsync()
        {
            return await Repository.ReadAllAsync(CompanyFilter);
        }

        /// <summary>
        /// Asynchronously read all the elements in the
        ///     table  <see cref="ContactAddress"/> for the current Contact        
        ///</summary>
        /// <param name="contactId"></param>
        /// <returns>A list with all the ContactAddress given ContactId</returns>
        public async Task<IEnumerable<ContactAddress>> ReadAllAddressAsync(int contactId)
        {
            var address = await ContactAddressesRepository.ReadAllAsync(
                                entity => entity.ContactId == contactId);

            return address;
        }

        /// <summary>
        ///     Adds an object to the table <see cref="ContactAddress"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<ContactAddress> AddContactAddressAsync(ContactAddress obj)
        {
            return await ContactAddressesRepository.AddAsync(obj);
        }

        public async Task<ContactAddress> UpdateContactAddressAsync(ContactAddress obj)
        {
            return await ContactAddressesRepository.UpdateAsync(obj);
        }

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public async Task<Domain.Models.Address> UpdateAddressAsync(Domain.Models.Address obj)
        {
            await this.UpdateCoordinatesAsync(obj);
            return await AddressesRepository.UpdateAsync(obj);
        }

        public async Task<Domain.Models.Address> AddAddressAsync(Domain.Models.Address obj)
        {
            await this.UpdateCoordinatesAsync(obj);
            return await AddressesRepository.AddAsync(obj);
        }

        public async Task<int> PopulateEmptyCoordinatesAsync()
        {
            var dbContext = this.DbContext as MGCapDbContext;
            var addresses = dbContext.Addresses.ToList(); //Where(ad => ad.Longitude == null || ad.Latitude == null);
            foreach (var address in addresses)
            {
                try
                {
                    await this.UpdateCoordinatesAsync(address);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                }
            }

            var toUpdate = addresses;//.Where(ad => ad.Longitude.HasValue && ad.Longitude.HasValue);
            if (toUpdate.Any())
            {
                await this.AddressesRepository.UpdateRangeAsync(toUpdate);
                return toUpdate.Count();
            }
            return 0;
        }

        private async Task UpdateCoordinatesAsync(Domain.Models.Address obj)
        {
            try
            {
                var geocoder = this.Geocoder as GoogleGeocoder;
                var addresses = await geocoder.GeocodeAsync(obj.FullAddress);
                if (addresses.Count() == 1)
                {
                    var gAddress = addresses.First();
                    obj.Latitude = gAddress.Coordinates.Latitude;
                    obj.Longitude = gAddress.Coordinates.Longitude;

                    var loader = new Dictionary<GoogleAddressType, string>
                    {
                        [GoogleAddressType.Locality] = "",
                        [GoogleAddressType.AdministrativeAreaLevel1] = "",
                        [GoogleAddressType.PostalCode] = "",
                        [GoogleAddressType.StreetNumber] = "",
                        [GoogleAddressType.Route] = "",
                        [GoogleAddressType.Subpremise] = "",
                        [GoogleAddressType.Country] = "",
                    };

                    foreach (var key in loader.Keys.ToList())
                    {
                        try
                        {
                            loader[key] = gAddress[key].ShortName;
                        }
                        catch (Exception)
                        {
                            // ASK FORGIVENESS
                            continue;
                        }
                    }

                    var streetNumber = loader[GoogleAddressType.StreetNumber];
                    var route = loader[GoogleAddressType.Route];
                    var subpremise = loader[GoogleAddressType.Subpremise];
                    subpremise = string.IsNullOrEmpty(subpremise) ? string.Empty : $"#{subpremise}";
                    route = string.IsNullOrEmpty(route) ? string.Empty : $" {route}";
                    obj.AddressLine1 = $"{streetNumber}{route}";
                    obj.AddressLine2 = subpremise;
                    obj.City = loader[GoogleAddressType.Locality];
                    obj.State = loader[GoogleAddressType.AdministrativeAreaLevel1];
                    obj.CountryCode = loader[GoogleAddressType.Country];
                }
            }
            catch (GoogleGeocodingException gEx)
            {
#if DEBUG
                Console.WriteLine(gEx.Message);
#endif
            }
            catch (ArgumentException aEx)
            {
#if DEBUG
                Console.WriteLine(aEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
            }
        }

        public void RemoveAddress(ContactAddress obj)
        {
            ContactAddressesRepository.Remove(obj);
        }

        public void RemoveAddresses(int contactId)
        {
            ContactAddressesRepository.Remove(ent => ent.ContactId == contactId);
        }

        public Task<ContactAddress> GetContactAddressByIdAsync(int contactId, int addressId)
        {
            return ContactAddressesRepository.SingleOrDefaultAsync(ent => ent.ContactId == contactId && ent.AddressId == addressId);
        }

        #region Contact's Related
        //Customer's
        public Task<CustomerContact> AddCustomerContact(CustomerContact obj)
        {
            return CustomerContactsRepository.AddAsync(obj);
        }

        public void RemoveCustomerContact(CustomerContact obj)
        {
            CustomerContactsRepository.Remove(obj);
        }

        public Task<CustomerContact> UpdateCustomerContact(CustomerContact obj)
        {
            return CustomerContactsRepository.UpdateAsync(obj);
        }

        public Task<CustomerContact> GetContactCustomerByIdAsync(int contactId, int customerId)
        {
            return CustomerContactsRepository.SingleOrDefaultAsync(ent => ent.ContactId == contactId && ent.CustomerId == customerId);
        }

        //Vendor's
        public Task<VendorContact> AddVendorContact(VendorContact obj)
        {
            return VendorContactsRepository.AddAsync(obj);
        }

        public void RemoveVendorContact(VendorContact obj)
        {
            VendorContactsRepository.Remove(obj);
        }

        public Task<VendorContact> UpdateVendorContact(VendorContact obj)
        {
            return VendorContactsRepository.UpdateAsync(obj);
        }

        public Task<VendorContact> GetContactVendorByIdAsync(int contactId, int vendorId)
        {
            return VendorContactsRepository.SingleOrDefaultAsync(ent => ent.ContactId == contactId && ent.VendorId == vendorId);
        }

        public async Task<DataSource<ContactGridViewModel>> ReadAllCustomerContactsDapperAsync(DataSourceRequest request, int customerId)
        {
            return await CustomerContactsRepository.ReadAllAsyncDapper(request, customerId);
        }

        public async Task<DataSource<ContactGridViewModel>> ReadAllVendorContactsDapperAsync(DataSourceRequest request, int vendorId)
        {
            return await VendorContactsRepository.ReadAllAsyncDapper(request, vendorId);
        }

        public async Task<DataSource<ContactLogsViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return await this.Repository.ReadAllDapperAsync(request, this.CompanyId);
        }

        public Task<DataSource<ContactListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId, id);
        }

        public async Task<DataSource<ContactGridViewModel>> ReadAllBuildingContactsDapperAsync(DataSourceRequest request, int buildingId)
        {
            return await this.BuildingContactsRepository.ReadAllAsyncDapper(request, buildingId);
        }

        public Task<BuildingContact> GetContactBuildingByIdAsync(int contactId, int buildingId)
        {
            return this.BuildingContactsRepository.SingleOrDefaultAsync(ent => ent.ContactId == contactId && ent.BuildingId == buildingId);
        }

        public void RemoveBuildingContact(BuildingContact obj)
        {
            this.BuildingContactsRepository.Remove(obj);
        }

        public Task<BuildingContact> AddBuildingContactAsync(BuildingContact obj)
        {
            return this.BuildingContactsRepository.AddAsync(obj);
        }

        public Task<BuildingContact> UpdateBuildingContactAsync(BuildingContact obj)
        {
            return this.BuildingContactsRepository.UpdateAsync(obj);
        }

        public Task<DataSource<ListBoxViewModel>> ReadAllBldgContactCboDapperAsync(DataSourceRequest request, int? id, int? buildingId, WorkOrderContactType type = null)
        {
            return this.BuildingContactsRepository.ReadAllCboDapperAsync(request, CompanyId, id, buildingId, type);
        }
        #endregion
    }
}
