using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Presentation.ViewModels.DataViewModels.EntityPhone;
using MGCap.Presentation.ViewModels.DataViewModels.EntityEmail;
using MGCap.Presentation.ViewModels.DataViewModels.Contact;
using MGCap.Presentation.ViewModels.DataViewModels.Address;
using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Data;
using MGCap.Presentation.Filters;
using MGCap.Domain.ViewModels.Employee;

namespace MGCap.Presentation.Controllers
{
    public class ContactsController : BaseController
    {
        private readonly IContactsApplicationService _contactsApplicationService;

        public ContactsController(
            IEmployeesApplicationService employeeAppService,
            IContactsApplicationService contactsApplicationService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            _contactsApplicationService = contactsApplicationService;
        }

        #region Phone Related
        // GET: api/contacts/readallphones/<contactId>
        [HttpGet("{contactId:int}")]
        public async Task<JsonResult> ReadAllPhones(int contactId)
        {
            var phonesObj = await _contactsApplicationService.ReadAllPhonesAsync(contactId);
            var phonesVMs = this.Mapper.Map<IEnumerable<ContactPhone>, IEnumerable<EntityPhoneGridViewModel>>(phonesObj);
            return new JsonResult(phonesVMs);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhone([FromBody] EntityPhoneCreateViewModel phonesVM)
        {
            if (this.ModelState.IsValid)
            {
                if (phonesVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var phonesObj = this.Mapper.Map<EntityPhoneCreateViewModel, ContactPhone>(phonesVM);
                await this._contactsApplicationService.AddPhoneAsync(phonesObj);
                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{contactPhoneId:int}")]
        public async Task<IActionResult> UpdatePhone(int contactPhoneId)
        {
            var phonesObj = await _contactsApplicationService.GetPhoneByIdAsync(contactPhoneId);
            if (phonesObj == null)
            {
                return this.NotFound();
            }

            var phonesVMs = this.Mapper.Map<ContactPhone, EntityPhoneUpdateViewModel>(phonesObj);

            return new JsonResult(phonesVMs);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePhone([FromBody] EntityPhoneUpdateViewModel phonesVM)
        {
            if (this.ModelState.IsValid)
            {
                if (phonesVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var phonesObj = await _contactsApplicationService.GetPhoneByIdAsync(phonesVM.ID);
                if (phonesObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(phonesVM, phonesObj);
                await this._contactsApplicationService.UpdatePhoneAsync(phonesObj);

                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{contactPhoneId:int}")]
        public async Task<IActionResult> DeletePhone(int contactPhoneId)
        {
            var phonesObj = await _contactsApplicationService.GetPhoneByIdAsync(contactPhoneId);
            if (phonesObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _contactsApplicationService.RemovePhone(contactPhoneId);
            await this._contactsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Email Related
        // GET: api/contacts/readallemails/<contactId>
        [HttpGet("{contactId:int}")]
        public async Task<JsonResult> ReadAllEmails(int contactId)
        {
            var emailsObj = await _contactsApplicationService.ReadAllEmailsAsync(contactId);
            var emailsVMs = this.Mapper.Map<IEnumerable<ContactEmail>, IEnumerable<EntityEmailGridViewModel>>(emailsObj);
            return new JsonResult(emailsVMs = this.Mapper.Map<IEnumerable<ContactEmail>, IEnumerable<EntityEmailGridViewModel>>(emailsObj));
        }

        [HttpPost]
        public async Task<IActionResult> AddEmail([FromBody] EntityEmailCreateViewModel emailVM)
        {
            if (this.ModelState.IsValid)
            {
                if (emailVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var emailObj = this.Mapper.Map<EntityEmailCreateViewModel, ContactEmail>(emailVM);
                await this._contactsApplicationService.AddEmailAsync(emailObj);
                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{contactEmailId:int}")]
        public async Task<IActionResult> UpdateEmail(int contactEmailId)
        {
            var emailObj = await _contactsApplicationService.GetEmailByIdAsync(contactEmailId);
            if (emailObj == null)
            {
                return this.NotFound();
            }

            var emailVMs = this.Mapper.Map<ContactEmail, EntityEmailUpdateViewModel>(emailObj);

            return new JsonResult(emailVMs);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmail([FromBody] EntityEmailUpdateViewModel emailVM)
        {
            if (this.ModelState.IsValid)
            {
                if (emailVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var emailObj = await _contactsApplicationService.GetEmailByIdAsync(emailVM.ID);
                if (emailObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(emailVM, emailObj);
                await this._contactsApplicationService.UpdateEmailAsync(emailObj);

                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{contactEmailId:int}")]
        public async Task<IActionResult> DeleteEmail(int contactEmailId)
        {
            var emailObj = await _contactsApplicationService.GetEmailByIdAsync(contactEmailId);
            if (emailObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _contactsApplicationService.RemoveEmail(contactEmailId);
            await this._contactsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Contact Related
        // GET: api/contacts/readall
        [HttpGet]
        [PermissionsFilter("ReadContacts")]
        public async Task<JsonResult> ReadAll(DataSourceRequest request)
        {
            var dataSource = await this._contactsApplicationService.ReadAllDapperAsync(request);
            return new JsonResult(dataSource);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id)
        {
            DataSource<ContactListBoxViewModel> dataSource = await this._contactsApplicationService.ReadAllCboDapperAsync(request, id);
            return new JsonResult(dataSource);
        }

        [HttpGet("{guid:Guid}")]
        [PermissionsFilter("ReadContacts")]
        public async Task<IActionResult> Update(Guid guid)
        {
            var contactObj = await _contactsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (contactObj == null)
            {
                return this.NotFound();
            }

            var contactVMs = this.Mapper.Map<Contact, ContactUpdateViewModel>(contactObj);

            return new JsonResult(contactVMs);
        }

        [HttpPut]
        [PermissionsFilter("UpdateContacts")]
        public async Task<IActionResult> Update([FromBody] ContactUpdateViewModel contactVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contactVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var contactObj = await _contactsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == contactVM.Guid);
                if (contactObj == null)
                {
                    return BadRequest(this.ModelState);
                }
                this.Mapper.Map(contactVM, contactObj);
                await this._contactsApplicationService.UpdateAsync(contactObj);
                var type = this.GetType(contactVM.ContactType);
                if (type == ContactType.Customer)
                {
                    var customerContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactUpdateViewModel, CustomerContact>(contactVM);
                    //update CustomerContact
                    await _contactsApplicationService.UpdateCustomerContact(customerContactObj);
                }
                else if (type == ContactType.Vendor)
                {
                    var vendorContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactUpdateViewModel, VendorContact>(contactVM);
                    //update VendorContact
                    await _contactsApplicationService.UpdateVendorContact(vendorContactObj);
                }
                else if (type == ContactType.Building)
                {
                    var buildingContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactUpdateViewModel, BuildingContact>(contactVM);
                    //update VendorContact
                    await _contactsApplicationService.UpdateBuildingContactAsync(buildingContactObj);
                }


                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("AddContacts")]
        public async Task<IActionResult> Add([FromBody] ViewModels.DataViewModels.Contact.ContactCreateViewModel contactVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contactVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                try
                {
                    var contactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, Contact>(contactVM);
                    await this._contactsApplicationService.AddAsync(contactObj);
                    contactVM.ID = contactObj.ID;

                    // Adding Phone and Email if exists
                    if (!string.IsNullOrEmpty(contactVM.Phone))
                    {
                        var phoneObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, ContactPhone>(contactVM);

                        await this._contactsApplicationService.AddPhoneAsync(phoneObj);
                    }

                    if (!string.IsNullOrEmpty(contactVM.Email))
                    {
                        var emailObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, ContactEmail>(contactVM);

                        await this._contactsApplicationService.AddEmailAsync(emailObj);
                    }

                    await this._contactsApplicationService.SaveChangesAsync();
                    var result = this.Mapper.Map<Contact, ContactDetailsViewModel>(contactObj);

                    return new JsonResult(result);
                }
                catch (DuplicateNameException dnEx) // only when email already exists
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, dnEx.Message);
                }
                catch (DbUpdateException) // for another duplicated key not implemented yet
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, "You are trying to create a record that already exists.");
                }
                catch (Exception) // unexpected exception
                {
                    return this.BadRequest(this.ModelState);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("AssignContacts")]
        public async Task<IActionResult> Assign([FromBody] ViewModels.DataViewModels.Contact.ContactCreateViewModel contactVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contactVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var contactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, Contact>(contactVM);

                //Verifying if is a new Contact
                if (contactObj.ID == 0)
                {
                    await this._contactsApplicationService.AddAsync(contactObj);
                    contactVM.ID = contactObj.ID;
                }

                // Updating VM's ID
                contactVM.ID = contactObj.ID;

                // Adding Phone and Email if exists
                if (!string.IsNullOrEmpty(contactVM.Phone))
                {
                    var phoneObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, ContactPhone>(contactVM);

                    await this._contactsApplicationService.AddPhoneAsync(phoneObj);
                }

                if (!string.IsNullOrEmpty(contactVM.Email))
                {
                    var emailObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, ContactEmail>(contactVM);

                    await this._contactsApplicationService.AddEmailAsync(emailObj);
                }

                var type = this.GetType(contactVM.ContactType);
                if (type == ContactType.Customer)
                {
                    var customerContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, CustomerContact>(contactVM);
                    //create CustomerContact
                    await _contactsApplicationService.AddCustomerContact(customerContactObj);
                }
                else if (type == ContactType.Vendor)
                {
                    var vendorContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, VendorContact>(contactVM);
                    //create VendorContact
                    await _contactsApplicationService.AddVendorContact(vendorContactObj);
                }
                else if (type == ContactType.Building)
                {
                    var buildingContactObj = this.Mapper.Map<ViewModels.DataViewModels.Contact.ContactCreateViewModel, BuildingContact>(contactVM);
                    //create VendorContact
                    await _contactsApplicationService.AddBuildingContactAsync(buildingContactObj);
                }
                await this._contactsApplicationService.SaveChangesAsync();

                var result = this.Mapper.Map<Contact, ContactUpdateViewModel>(contactObj);
                return new JsonResult(result);
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("AssignContacts")]
        public async Task<IActionResult> AssignContact([FromBody] ContactAssignViewModel contactVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contactVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                var contactObj = await this._contactsApplicationService.SingleOrDefaultAsync(ent => ent.ID == contactVM.ContactId);

                var type = contactVM.ContactType;
                try
                {
                    if (type == ContactType.Customer)
                    {
                        var customerContactObj = this.Mapper.Map<ContactAssignViewModel, CustomerContact>(contactVM);
                        //create CustomerContact
                        await _contactsApplicationService.AddCustomerContact(customerContactObj);
                    }
                    else if (type == ContactType.Vendor)
                    {
                        var vendorContactObj = this.Mapper.Map<ContactAssignViewModel, VendorContact>(contactVM);
                        //create VendorContact
                        await _contactsApplicationService.AddVendorContact(vendorContactObj);
                    }
                    else if (type == ContactType.Building)
                    {
                        var buildingContactObj = this.Mapper.Map<ContactAssignViewModel, BuildingContact>(contactVM);
                        //create VendorContact
                        await _contactsApplicationService.AddBuildingContactAsync(buildingContactObj);
                    }
                    await this._contactsApplicationService.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    if (((System.Data.SqlClient.SqlException)dbEx.InnerException).Number == 2627)
                    {
                        return StatusCode(
                            (int)HttpStatusCode.PreconditionFailed,
                            $"The selected contact is assigned to the {type} already, please, select or create another contact."
                            );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var result = this.Mapper.Map<Contact, ContactDetailsViewModel>(contactObj);
                return new JsonResult(result);
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{contactId:int}")]
        [PermissionsFilter("UnassignContacts")]
        public async Task<IActionResult> Unassign(int contactId, ContactDeleteViewModel contactVM)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error_code = ErrorCode.BadRequest,
                    error_message = this.ModelState
                });
            }
            var type = this.GetType(contactVM.ContactType);
            if (type == ContactType.Customer)
            {
                var contactCustomerObj = await _contactsApplicationService.GetContactCustomerByIdAsync(contactId, contactVM.EntityId);
                if (contactCustomerObj == null)
                {
                    return this.NotFound(new { error_code = 4004, error_message = $"The contact with ContactId: {contactId} and CustomerId: {contactVM.EntityId} doesn't exist" });
                }
                _contactsApplicationService.RemoveCustomerContact(contactCustomerObj);
            }
            else if (type == ContactType.Vendor)
            {
                //Remove VendorContact 
                var contactVendorObj = await _contactsApplicationService.GetContactVendorByIdAsync(contactId, contactVM.EntityId);
                if (contactVendorObj == null)
                {
                    return this.NotFound(new { error_code = 4004, error_message = $"The contact with ContactId: {contactId} and VendorId: {contactVM.EntityId} doesn't exist" });
                }
                _contactsApplicationService.RemoveVendorContact(contactVendorObj);
            }
            else if (type == ContactType.Building)
            {
                //Remove VendorContact 
                var contactBuildingObj = await _contactsApplicationService.GetContactBuildingByIdAsync(contactId, contactVM.EntityId);
                if (contactBuildingObj == null)
                {
                    return this.NotFound(new { error_code = 4004, error_message = $"The contact with ContactId: {contactId} and BuildingId: {contactVM.EntityId} doesn't exist" });
                }
                _contactsApplicationService.RemoveBuildingContact(contactBuildingObj);
            }
            else
            {
                return BadRequest(new
                {
                    error_code = ErrorCode.BadRequest,
                    error_message = $"Invalid ContactType: {contactVM.ContactType}"
                });
            }

            await this._contactsApplicationService.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{guid:Guid}")]
        [PermissionsFilter("DeleteContacts")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var contactObj = await _contactsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (contactObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _contactsApplicationService.Remove(contactObj.ID);
            await this._contactsApplicationService.SaveChangesAsync();
            return Ok();
        }

        [PermissionsFilter("DeleteContacts")]
        [HttpDelete]
        public async Task<IActionResult> DeleteByGuid(Guid guid)
        {
            var result = new EmployeeDeteleResponseViewModel();

            var customer = await this._contactsApplicationService.SingleOrDefaultAsync(e => e.Guid.Equals(guid));
            if (customer == null)
            {
                result.Success = false;
                result.Message = "The customer doesn't exist";
            }

            string username = customer == null ? string.Empty : customer.FullName;

            try
            {
                await this._contactsApplicationService.RemoveAsync(customer);
                await this._contactsApplicationService.SaveChangesAsync();

                result.Success = true;
                result.Message = $"The customer ({username}) was deleted succesfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Delete customer ({username}) not allowed";
            }

            return Ok(result);
        }
        
        // GET: api/contacts/get/<guid>
        [HttpGet("{guid:Guid}")]
        [PermissionsFilter("ReadContacts")]
        public async Task<JsonResult> Get(Guid guid)
        {
            var contactObj = await _contactsApplicationService.SingleOrDefaultAsync(entity => entity.Guid == guid);
            // TODO : Use a different view model
            var contactVM = this.Mapper.Map<Contact, ContactDetailsViewModel>(contactObj);
            return new JsonResult(contactVM);
        }

        // GET: api/contacts/get/<guid>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadContacts")]
        public async Task<JsonResult> Get(int id)
        {
            var contactObj = await _contactsApplicationService.SingleOrDefaultAsync(entity => entity.ID == id);
            // TODO : Use a different view model
            var contactVM = this.Mapper.Map<Contact, ContactDetailsViewModel>(contactObj);
            return new JsonResult(contactVM);
        }

        [HttpGet("{contactId:int}")]
        public async Task<IActionResult> GetContact(int contactId, int entityId, string contactType)
        {
            if (contactType == null || contactType == "")
                return BadRequest(ModelState);
            var type = this.GetType(contactType);
            if (type == ContactType.Customer)
            {
                var contactObj = await _contactsApplicationService.GetContactCustomerByIdAsync(contactId, entityId);
                var contactVM = this.Mapper.Map<CustomerContact, ContactDetailsViewModel>(contactObj);
                return new JsonResult(contactVM);
            }
            if (type == ContactType.Vendor)
            {
                var contactObj = await _contactsApplicationService.GetContactVendorByIdAsync(contactId, entityId);
                var contactVM = this.Mapper.Map<VendorContact, ContactDetailsViewModel>(contactObj);
                return new JsonResult(contactVM);
            }
            if (type == ContactType.Building)
            {
                var contactObj = await _contactsApplicationService.GetContactBuildingByIdAsync(contactId, entityId);
                var contactVM = this.Mapper.Map<BuildingContact, ContactDetailsViewModel>(contactObj);
                return new JsonResult(contactVM);
            }
            return BadRequest(new
            {
                error_code = ErrorCode.BadRequest,
                error_message = $"Invalid ContactType: {contactType}"
            });
        }

        // GET: api/customers/readall
        [HttpGet("{entityId:int}")]
        public async Task<IActionResult> ReadAllContacts(DataSourceRequest request, int entityId, string contactType)
        {
            var dataSource = new DataSource<Domain.ViewModels.Contact.ContactGridViewModel>();
            if (contactType == null || contactType == "")
            {
                return BadRequest(new
                {
                    error_code = ErrorCode.BadRequest,
                    error_message = $"Empty ContactType"
                });
            }
            var type = this.GetType(contactType);
            switch (type)
            {
                case ContactType.Customer:
                    dataSource = await _contactsApplicationService.ReadAllCustomerContactsDapperAsync(request, entityId);
                    break;
                case ContactType.Vendor:
                    dataSource = await this._contactsApplicationService.ReadAllVendorContactsDapperAsync(request, entityId);
                    break;
                case ContactType.Building:
                    dataSource = await this._contactsApplicationService.ReadAllBuildingContactsDapperAsync(request, entityId);
                    break;
                default:
                    return BadRequest(new
                    {
                        error_code = ErrorCode.BadRequest,
                        error_message = $"Invalid ContactType: {contactType}"
                    });
            }

            return new JsonResult(dataSource.Payload);
        }

        [HttpGet("{entityId:int}")]
        public async Task<IActionResult> ReadAllContactsBuildingProfile(DataSourceRequest request, int entityId, string contactType)
        {
            var dataSource = new DataSource<Domain.ViewModels.Contact.ContactGridViewModel>();
            if (contactType == null || contactType == "")
            {
                return BadRequest(new
                {
                    error_code = ErrorCode.BadRequest,
                    error_message = $"Empty ContactType"
                });
            }
            var type = this.GetType(contactType);
            switch (type)
            {
                case ContactType.Customer:
                    dataSource = await _contactsApplicationService.ReadAllCustomerContactsDapperAsync(request, entityId);
                    break;
                case ContactType.Vendor:
                    dataSource = await this._contactsApplicationService.ReadAllVendorContactsDapperAsync(request, entityId);
                    break;
                case ContactType.Building:
                    dataSource = await this._contactsApplicationService.ReadAllBuildingContactsDapperAsync(request, entityId);
                    break;
                default:
                    return BadRequest(new
                    {
                        error_code = ErrorCode.BadRequest,
                        error_message = $"Invalid ContactType: {contactType}"
                    });
            }

            return new JsonResult(dataSource.Payload);
        }


        [HttpGet]
        public async Task<JsonResult> ReadAllBldgContactCbo(DataSourceRequest request, int? id, int? buildingId, string contactType)
        {
            var type = String.IsNullOrEmpty(contactType) || String.IsNullOrWhiteSpace(contactType) ? null : new WorkOrderContactType(contactType);
            var contacts = await _contactsApplicationService.ReadAllBldgContactCboDapperAsync(request, id, buildingId, type);
            return new JsonResult(contacts);

        }
        #endregion

        /// <summary>
        ///     Performs a case insensitive comparison
        ///     between a contact type and all the types
        /// </summary>
        /// <param name="strType"></param>
        /// <returns>The actual contact type, 0 in case didn't match</returns>
        private ContactType GetType(string strType)
        {
            foreach (ContactType type in Enum.GetValues(typeof(ContactType)))
            {
                if (string.Equals(strType, type.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return type;
                }
            }
            return ContactType.Undefined;
        }

        #region Address Related
        // GET: api/contacts/readalladdresses/<contactId>
        [HttpGet("{contactId:int}")]
        public async Task<JsonResult> ReadAllAddresses(int contactId)
        {
            var addressesObj = await _contactsApplicationService.ReadAllAddressAsync(contactId);
            var addressesVMs = this.Mapper.Map<IEnumerable<ContactAddress>, IEnumerable<AddressGridViewModel>>(addressesObj);
            return new JsonResult(addressesVMs);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressCreateViewModel addressVM)
        {
            if (this.ModelState.IsValid)
            {
                if (addressVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var addressObj = this.Mapper.Map<AddressCreateViewModel, Address>(addressVM);
                var contactAddressObj = this.Mapper.Map<AddressCreateViewModel, ContactAddress>(addressVM);
                await this._contactsApplicationService.AddAddressAsync(addressObj);
                contactAddressObj.AddressId = addressObj.ID;
                await this._contactsApplicationService.AddContactAddressAsync(contactAddressObj);
                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{contactId:int}")]
        public async Task<IActionResult> UpdateAddress(int contactId, int addressId)
        {
            var contactAddressObj = await _contactsApplicationService.GetContactAddressByIdAsync(contactId, addressId);
            if (contactAddressObj == null)
            {
                return this.NotFound();
            }

            var AddressVMs = this.Mapper.Map<ContactAddress, AddressUpdateViewModel>(contactAddressObj);

            return new JsonResult(AddressVMs);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateViewModel AddressVM)
        {
            if (this.ModelState.IsValid)
            {
                if (AddressVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var contactAddressObj = await _contactsApplicationService.GetContactAddressByIdAsync(AddressVM.EntityId, AddressVM.AddressId);
                if (contactAddressObj == null)
                {
                    return BadRequest(this.ModelState);
                }
                var addressObj = contactAddressObj.Address;

                this.Mapper.Map(AddressVM, contactAddressObj);
                this.Mapper.Map(AddressVM, addressObj);
                await this._contactsApplicationService.UpdateContactAddressAsync(contactAddressObj);
                await this._contactsApplicationService.UpdateAddressAsync(addressObj);

                await this._contactsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{contactId:int}")]
        public async Task<IActionResult> DeleteAddress(int contactId, int addressId)
        {
            var addressObj = await _contactsApplicationService.GetContactAddressByIdAsync(contactId, addressId);
            if (addressObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _contactsApplicationService.RemoveAddress(addressObj);
            await this._contactsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Update Addresses
        [HttpGet]
        public async Task<IActionResult> PopulateEmptyCoordinates()
        {
            var count = await this._contactsApplicationService.PopulateEmptyCoordinatesAsync();
            await this._contactsApplicationService.SaveChangesAsync();
            return new JsonResult(new { affectedAddresses = count });
        }
        #endregion
    }
}
