using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Presentation.ViewModels.DataViewModels.Address;
using MGCap.Presentation.ViewModels.DataViewModels.Contact;
using MGCap.Presentation.ViewModels.DataViewModels.EntityEmail;
using MGCap.Presentation.ViewModels.DataViewModels.EntityPhone;
using MGCap.Presentation.ViewModels.DataViewModels.Vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class VendorsController : BaseController
    {
        private readonly IVendorsApplicationService _vendorsApplicationService;
        private readonly IContactsApplicationService _contactsApplicationService;

        public VendorsController(
            IEmployeesApplicationService employeeAppService,
            IVendorsApplicationService vendorsApplicationService,
            IContactsApplicationService contactApplicationService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            _vendorsApplicationService = vendorsApplicationService;
            _contactsApplicationService = contactApplicationService;
        }

        #region Vendors
        // GET: api/vendors/readall
        [HttpGet]
        public async Task<JsonResult> ReadAll(DataSourceRequest request)
        {
            var dataSource = await this._vendorsApplicationService.ReadAllDapperAsync(request);
            return new JsonResult(dataSource);
        }

        [HttpGet("{guid:Guid}")]
        public async Task<IActionResult> Update(Guid guid)
        {
            var vendorsObj = await _vendorsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (vendorsObj == null)
            {
                return this.NotFound();
            }

            var vendorsVMs = this.Mapper.Map<Vendor, VendorUpdateViewModel>(vendorsObj);

            return new JsonResult(vendorsVMs);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VendorUpdateViewModel vendorVM)
        {
            if (this.ModelState.IsValid)
            {
                if (vendorVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var vendorObj = await _vendorsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == vendorVM.Guid);
                if (vendorObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(vendorVM, vendorObj);
                await this._vendorsApplicationService.UpdateAsync(vendorObj);

                //Remove all current assigned groups
                this._vendorsApplicationService.RemoveAllVendorVendorGroups(vendorVM.ID);

                if (vendorVM.GroupIds != null)
                {
                    //Insert all groups
                    this._vendorsApplicationService.AssignVendorGroups(vendorVM.GroupIds, vendorVM.ID);
                }

                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(this.ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] VendorCreateViewModel vendorVM)
        {
            if (this.ModelState.IsValid)
            {
                if (vendorVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var vendorObj = this.Mapper.Map<VendorCreateViewModel, Vendor>(vendorVM);
                await this._vendorsApplicationService.AddAsync(vendorObj);
                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(this.ModelState);
        }

        [HttpDelete("{guid:Guid}")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var vendorObj = await _vendorsApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (vendorObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _vendorsApplicationService.Remove(vendorObj.ID);
            await this._vendorsApplicationService.SaveChangesAsync();
            return Ok();
        }

        // GET: api/vendors/get/<guid>
        [HttpGet("{guid:Guid}")]
        public async Task<IActionResult> Get(Guid guid)
        {
            if (guid == null)
            {
                return this.BadRequest(this.ModelState);
            }
            return await Get(entity => entity.Guid == guid);
        }

        // GET: api/vendors/get/<id>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            return await Get(entity => entity.ID == id);
        }

        /// <summary>
        ///     Common method for retrieving vendor info
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<IActionResult> Get(Func<Vendor, bool> func)
        {
            var vendorObj = await _vendorsApplicationService.SingleOrDefaultAsync(func);
            // TODO : Use a different view model
            var vendorVM = this.Mapper.Map<Vendor, VendorDetailsViewModel>(vendorObj);
            return new JsonResult(vendorVM);
        }

        // GET: api/vendors/getall
        [HttpGet("{id:int?}")]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id)
        {
            var vendorsVM = await _vendorsApplicationService.ReadAllCboAsyncDapper(request, id);
            return new JsonResult(vendorsVM);
        }
        #endregion

        #region Phone
        // GET: api/vendors/readallphones/<vendorId>
        [HttpGet("{vendorId:int}")]
        public async Task<JsonResult> ReadAllPhones(int vendorId)
        {
            var phonesObj = await _vendorsApplicationService.ReadAllPhonesAsync(vendorId);
            var phonesVMs = this.Mapper.Map<IEnumerable<VendorPhone>, IEnumerable<EntityPhoneGridViewModel>>(phonesObj);
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
                var phonesObj = this.Mapper.Map<EntityPhoneCreateViewModel, VendorPhone>(phonesVM);
                await this._vendorsApplicationService.AddPhoneAsync(phonesObj);
                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(this.ModelState);
        }


        [HttpGet("{vendorPhoneId:int}")]
        public async Task<IActionResult> UpdatePhone(int vendorPhoneId)
        {
            var phonesObj = await _vendorsApplicationService.GetPhoneByIdAsync(vendorPhoneId);
            if (phonesObj == null)
            {
                return this.NotFound();
            }

            var phonesVMs = this.Mapper.Map<VendorPhone, EntityPhoneUpdateViewModel>(phonesObj);

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
                var phonesObj = await _vendorsApplicationService.GetPhoneByIdAsync(phonesVM.ID);
                if (phonesObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(phonesVM, phonesObj);
                await this._vendorsApplicationService.UpdatePhoneAsync(phonesObj);

                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{vendorPhoneId:int}")]
        public async Task<IActionResult> DeletePhone(int vendorPhoneId)
        {
            var phonesObj = await _vendorsApplicationService.GetPhoneByIdAsync(vendorPhoneId);
            if (phonesObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _vendorsApplicationService.RemovePhone(vendorPhoneId);
            await this._vendorsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Email Related
        // GET: api/vendors/readallemails/<vendorId>
        [HttpGet("{vendorId:int}")]
        public async Task<JsonResult> ReadAllEmail(int vendorId)
        {
            var emailsObj = await _vendorsApplicationService.ReadAllEmailsAsync(vendorId);
            var emailsVMs = this.Mapper.Map<IEnumerable<VendorEmail>, IEnumerable<EntityEmailGridViewModel>>(emailsObj);
            return new JsonResult(emailsVMs = this.Mapper.Map<IEnumerable<VendorEmail>, IEnumerable<EntityEmailGridViewModel>>(emailsObj));
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
                var emailObj = this.Mapper.Map<EntityEmailCreateViewModel, VendorEmail>(emailVM);
                await this._vendorsApplicationService.AddEmailAsync(emailObj);
                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{vendorEmailId:int}")]
        public async Task<IActionResult> UpdateEmail(int vendorEmailId)
        {
            var emailObj = await _vendorsApplicationService.GetEmailByIdAsync(vendorEmailId);
            if (emailObj == null)
            {
                return this.NotFound();
            }

            var emailVMs = this.Mapper.Map<VendorEmail, EntityEmailUpdateViewModel>(emailObj);

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
                var emailObj = await _vendorsApplicationService.GetEmailByIdAsync(emailVM.ID);
                if (emailObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(emailVM, emailObj);
                await this._vendorsApplicationService.UpdateEmailAsync(emailObj);

                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{vendorEmailId:int}")]
        public async Task<IActionResult> DeleteEmail(int vendorEmailId)
        {
            var emailObj = await _vendorsApplicationService.GetEmailByIdAsync(vendorEmailId);
            if (emailObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _vendorsApplicationService.RemoveEmail(vendorEmailId);
            await this._vendorsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion+

        #region Address Related
        // GET: api/vendors/readalladdresses/<vendorId>
        [HttpGet("{vendorId:int}")]
        public async Task<JsonResult> ReadAllAddresses(int vendorId)
        {
            var addressesObj = await _vendorsApplicationService.ReadAllAddressAsync(vendorId);
            var addressesVMs = this.Mapper.Map<IEnumerable<VendorAddress>, IEnumerable<AddressGridViewModel>>(addressesObj);
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
                var contactAddressObj = this.Mapper.Map<AddressCreateViewModel, VendorAddress>(addressVM);
                await this._vendorsApplicationService.AddAddressAsync(addressObj);
                contactAddressObj.AddressId = addressObj.ID;
                await this._vendorsApplicationService.AddContactAddressAsync(contactAddressObj);
                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{vendorId:int}")]
        public async Task<IActionResult> UpdateAddress(int vendorId, int addressId)
        {
            var contactAddressObj = await _vendorsApplicationService.GetVendorAddressByIdAsync(vendorId, addressId);
            if (contactAddressObj == null)
            {
                return this.NotFound();
            }

            var AddressVMs = this.Mapper.Map<VendorAddress, AddressUpdateViewModel>(contactAddressObj);

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
                var contactAddressObj = await _vendorsApplicationService.GetVendorAddressByIdAsync(AddressVM.EntityId, AddressVM.AddressId);
                if (contactAddressObj == null)
                {
                    return BadRequest(this.ModelState);
                }
                var addressObj = contactAddressObj.Address;

                this.Mapper.Map(AddressVM, contactAddressObj);
                this.Mapper.Map(AddressVM, addressObj);
                await this._vendorsApplicationService.UpdateContactAddressAsync(contactAddressObj);
                await this._vendorsApplicationService.UpdateAddressAsync(addressObj);

                await this._vendorsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{vendorId:int}")]
        public async Task<IActionResult> DeleteAddress(int vendorId, int addressId)
        {
            var addressObj = await _vendorsApplicationService.GetVendorAddressByIdAsync(vendorId, addressId);
            if (addressObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _vendorsApplicationService.RemoveAddress(addressObj);
            await this._vendorsApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Contact
        // GET: api/vendors/readallcontacts/<vendorId>
        //[HttpGet("api/vendors/readallcontacts/{vendorId:int}")]
        //public async Task<JsonResult> ReadAllContacts(int vendorId)
        //{
        //    var contactsObj = await _vendorsApplicationService.ReadAllContactsAsync(vendorId);
        //    var contactsVMs = this.Mapper.Map<IEnumerable<VendorContact>, IEnumerable<EntityContactGridViewModel>>(contactsObj);
        //    return new JsonResult(contactsVMs);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddContact([FromBody] EntityContactCreateViewModel vendorContactVM)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        if (vendorContactVM == null)
        //        {
        //            return BadRequest(this.ModelState);
        //        }
        //        var ContactObj = this.Mapper.Map<EntityContactCreateViewModel, Contact>(vendorContactVM);
        //        var vendorContactObj = this.Mapper.Map<EntityContactCreateViewModel, VendorContact>(vendorContactVM);
        //        await this._contactsApplicationService.AddAsync(ContactObj);
        //        vendorContactObj.ContactId = ContactObj.ID;
        //        await this._vendorsApplicationService.AddVendorContactAsync(vendorContactObj);
        //        await this._vendorsApplicationService.SaveChangesAsync();
        //        return Ok();
        //    }

        //    return this.BadRequest(this.ModelState);
        //}

        //[HttpDelete("api/vendors/deletecontact/{vendorId:int}")]
        //public async Task<IActionResult> DeleteContact(int vendorId, int contactId)
        //{
        //    var vendorContactObj = await _vendorsApplicationService.GetVendorContactByIdAsync(vendorId, contactId);
        //    if (vendorContactObj == null)
        //    {
        //        return BadRequest(this.ModelState);
        //    }
        //    _vendorsApplicationService.RemoveContact(vendorContactObj);
        //    await this._vendorsApplicationService.SaveChangesAsync();
        //    return Ok();
        //}
        #endregion

        #region VendorGroups
        // GET: api/vendors/readallgroups/
        [HttpGet("{vendorId:int}")]
        public async Task<JsonResult> ReadAllAssidnedGroups(int vendorId)
        {
            var vendorGroupObj = await _vendorsApplicationService.ReadAllVendorGroupAsync(vendorId);
            // TODO : Use a different view model
            var vendorVM = this.Mapper.Map<IEnumerable<VendorVendorGroup>, IEnumerable<ViewModels.Common.ListBoxViewModel>>(vendorGroupObj);
            return new JsonResult(vendorVM);
        }

        // GET: api/vendors/readallgroups/
        [HttpGet]
        public async Task<JsonResult> ReadAllGroups()
        {
            var vendorGroupObj = await _vendorsApplicationService.ReadAllVendorGroupAsync();
            // TODO : Use a different view model
            var vendorVM = this.Mapper.Map<IEnumerable<VendorGroup>, IEnumerable<ViewModels.Common.ListBoxViewModel>>(vendorGroupObj);
            return new JsonResult(vendorVM);
        }
        #endregion
    }
}
