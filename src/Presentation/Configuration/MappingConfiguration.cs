// -----------------------------------------------------------------------
// <copyright file="MappingConfiguration.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using AutoMapper;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Building;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.CleaningReportItem;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.Department;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Service;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Presentation.Extensions;
using MGCap.Presentation.ViewModels.AccountViewModels;
using MGCap.Presentation.ViewModels.DataViewModels.Address;
using MGCap.Presentation.ViewModels.DataViewModels.Contact;
using MGCap.Presentation.ViewModels.DataViewModels.Customer;
using MGCap.Presentation.ViewModels.DataViewModels.EntityEmail;
using MGCap.Presentation.ViewModels.DataViewModels.EntityPhone;
using MGCap.Presentation.ViewModels.DataViewModels.Vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Mobile;
using MGCap.Domain.ViewModels.OfficeServiceType;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ExpenseType;
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.Proposal;
using MGCap.Domain.ViewModels.ProposalService;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionItem;
using MGCap.Domain.ViewModels.Expense;
using MGCap.Domain.ViewModels.InspectionItemTicket;
using MGCap.Domain.ViewModels.InspectionItemTask;
using MGCap.Domain.ViewModels.Revenue;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using MGCap.Domain.ViewModels.PreCalendar;
using MGCap.Domain.ViewModels.CompanySettings;
using MGCap.Domain.ViewModels.Role;
using MGCap.Domain.ViewModels.ScheduleCategories;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using MGCap.Domain.ViewModels.CalendarItemFrequency;
using MGCap.Domain.ViewModels.ContractNote;
using MGCap.Domain.ViewModels.ContractActivityLogNote;
using MGCap.Domain.ViewModels.WorkOrderScheduleSetting;
using MGCap.Domain.ViewModels.Tag;
using MGCap.Domain.ViewModels.WorkOrderServiceCategory;
using MGCap.Domain.ViewModels.WorkOrderService;
using MGCap.Domain.ViewModels.WorkOrderTask;

namespace MGCap.Presentation.Configuration
{
    /// <summary>
    ///     For the mapping configuration of AutoMapper
    /// </summary>
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            // Please see https://github.com/AutoMapper/AutoMapper/wiki/Mapping-inheritance

            #region Employee/Contact Mappings
            this.CreateMap<Employee, EmployeeGridViewModel>()
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src => src.Contact.FullName))
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Contact.Phones.Where(p => p.Default).FirstOrDefault().Phone))
                .ForMember(
                    dest => dest.Ext,
                    opt => opt.MapFrom(src => src.Contact.Phones.Where(p => p.Default).FirstOrDefault().Ext))
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.Name));

            this.CreateMap<RegisterViewModel, Employee>();

            this.CreateMap<RegisterViewModel, Contact>();

            this.CreateMap<EmployeeUpdateViewModel, Employee>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.EmployeeStatusId);

            this.CreateMap<EmployeeUpdateViewModel, Contact>()
                .ForMember(
                        dest => dest.ID,
                        opt => opt.MapFrom(src => src.ContactId))
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.CompanyId);

            this.CreateMap<Employee, EmployeeUpdateViewModel>()
                .ForMember(
                        dest => dest.DOB,
                        opt => opt.MapFrom(src => src.Contact.DOB))
                .ForMember(
                        dest => dest.FirstName,
                        opt => opt.MapFrom(src => src.Contact.FirstName))
                .ForMember(
                        dest => dest.MiddleName,
                        opt => opt.MapFrom(src => src.Contact.MiddleName))
                .ForMember(
                        dest => dest.LastName,
                        opt => opt.MapFrom(src => src.Contact.LastName))
                .ForMember(
                        dest => dest.Gender,
                        opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(
                        dest => dest.Salutation,
                        opt => opt.MapFrom(src => src.Contact.Salutation))
                .ForMember(
                        dest => dest.SendNotifications,
                        opt => opt.MapFrom(src => src.Contact.SendNotifications))
                .ForMember(
                        dest => dest.Notes,
                        opt => opt.MapFrom(src => src.Contact.Notes))
                .ForMember(
                        dest => dest.RoleLevel,
                        opt => opt.MapFrom(src => src.Role.Level));

            this.CreateMap<Employee, EmployeeDetailsViewModel>()
                .ForMember(
                        dest => dest.DOB,
                        opt => opt.MapFrom(src => src.Contact.DOB))
                .ForMember(
                        dest => dest.FirstName,
                        opt => opt.MapFrom(src => src.Contact.FirstName))
                .ForMember(
                        dest => dest.MiddleName,
                        opt => opt.MapFrom(src => src.Contact.MiddleName))
                .ForMember(
                        dest => dest.LastName,
                        opt => opt.MapFrom(src => src.Contact.LastName))
                .ForMember(
                        dest => dest.Gender,
                        opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(
                        dest => dest.Salutation,
                        opt => opt.MapFrom(src => src.Contact.Salutation))
                .ForMember(
                        dest => dest.SendNotifications,
                        opt => opt.MapFrom(src => src.Contact.SendNotifications))
                .ForMember(
                        dest => dest.Notes,
                        opt => opt.MapFrom(src => src.Contact.Notes));

            this.CreateMap<Contact, ContactGridViewModel>()
                .ForMember(
                        dest => dest.Email,
                        opt => opt.MapFrom(src => src.Emails.FirstOrDefault(e => e.Default).Email))
                 .ForMember(
                        dest => dest.Phone,
                        opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                  .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(a => a.Default).Address.FullAddress));

            this.CreateMap<ContactUpdateViewModel, Contact>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Guid);

            this.CreateMap<Contact, ContactUpdateViewModel>();

            this.CreateMap<Contact, ContactDetailsViewModel>()
                .ForMember(
                        dest => dest.Email,
                        opt => opt.MapFrom(src => src.Emails.FirstOrDefault(e => e.Default).Email))
                 .ForMember(
                        dest => dest.Phone,
                        opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                  .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(a => a.Default).Address.FullAddress));

            this.CreateMap<ContactCreateViewModel, Contact>();
            #endregion

            #region Contact Phone Mapping
            this.CreateMap<ContactPhone, EntityPhoneGridViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<ContactPhone, EntityPhoneCreateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<EntityPhoneCreateViewModel, ContactPhone>()
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<ContactPhone, EntityPhoneDetailsViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<ContactPhone, EntityPhoneUpdateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<EntityPhoneUpdateViewModel, ContactPhone>()
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));
            this.CreateMap<ContactCreateViewModel, ContactPhone>()
                .IgnoreMember(dest => dest.ID)
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.ID))
                 .ForMember(
                        dest => dest.Type,
                        opt => opt.UseValue("Main Phone"))
                 .ForMember(
                        dest => dest.Default,
                        opt => opt.UseValue(true));
            #endregion

            #region  Contact Email Mapping
            this.CreateMap<ContactEmail, EntityEmailGridViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<ContactEmail, EntityEmailCreateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<EntityEmailCreateViewModel, ContactEmail>()
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<ContactEmail, EntityEmailDetailsViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<ContactEmail, EntityEmailUpdateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.ContactId));
            this.CreateMap<EntityEmailUpdateViewModel, ContactEmail>()
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));
            this.CreateMap<ContactCreateViewModel, ContactEmail>()
               .IgnoreMember(dest => dest.ID)
               .ForMember(
                       dest => dest.ContactId,
                       opt => opt.MapFrom(src => src.ID))
                .ForMember(
                       dest => dest.Type,
                       opt => opt.UseValue("Main Email"))
                .ForMember(
                       dest => dest.Default,
                       opt => opt.UseValue(true));
            #endregion

            #region ContactAddress Related Mapping
            this.CreateMap<ContactAddress, AddressBaseViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.ContactId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<AddressCreateViewModel, ContactAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<AddressUpdateViewModel, ContactAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.ContactId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<ContactAddress, AddressUpdateViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.ContactId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<ContactAddress, AddressDetailsViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.ContactId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));


            this.CreateMap<ContactAddress, AddressGridViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.ContactId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));
            #endregion

            #region Address Common mapping
            this.CreateMap<AddressCreateViewModel, Address>();

            this.CreateMap<AddressUpdateViewModel, Address>()
                .ForMember(
                        dest => dest.ID,
                        opt => opt.MapFrom(src => src.AddressId));
            #endregion

            #region Vendors Mapping
            this.CreateMap<Vendor, VendorGridViewModel>()
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                 .ForMember(
                    dest => dest.Ext,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Ext))
                .ForMember(
                        dest => dest.Email,
                        opt => opt.MapFrom(src => src.Emails.FirstOrDefault(e => e.Default).Email))
                 .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(e => e.Default).Address.FullAddress));

            this.CreateBothMaps<Vendor, VendorCreateViewModel>();

            this.CreateMap<Vendor, VendorDetailsViewModel>()
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                 .ForMember(
                    dest => dest.Ext,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Ext))
                .ForMember(
                        dest => dest.Email,
                        opt => opt.MapFrom(src => src.Emails.FirstOrDefault(e => e.Default).Email))
                 .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(e => e.Default).Address.FullAddress));

            this.CreateMap<Vendor, VendorUpdateViewModel>()
                .ForMember(dest => dest.GroupIds,
                            opt => opt.MapFrom(src => src.Groups.Select(ent => ent.VendorGroupId)));
            this.CreateMap<VendorUpdateViewModel, Vendor>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.VendorTypeId);
            #endregion

            #region Vendor Phone Mapping
            this.CreateMap<VendorPhone, EntityPhoneGridViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<VendorPhone, EntityPhoneCreateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<EntityPhoneCreateViewModel, VendorPhone>()
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<VendorPhone, EntityPhoneDetailsViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<VendorPhone, EntityPhoneUpdateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<EntityPhoneUpdateViewModel, VendorPhone>()
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));
            #endregion

            #region Vendor Email Mapping
            this.CreateMap<VendorEmail, EntityEmailGridViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<VendorEmail, EntityEmailCreateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<EntityEmailCreateViewModel, VendorEmail>()
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<VendorEmail, EntityEmailDetailsViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<VendorEmail, EntityEmailUpdateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.VendorId));
            this.CreateMap<EntityEmailUpdateViewModel, VendorEmail>()
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));
            #endregion

            #region VendorAddress related mapping
            this.CreateMap<VendorAddress, AddressBaseViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.VendorId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<AddressCreateViewModel, VendorAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<AddressUpdateViewModel, VendorAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.VendorId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<VendorAddress, AddressUpdateViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.VendorId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));


            this.CreateMap<VendorAddress, AddressDetailsViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.VendorId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<VendorAddress, AddressGridViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.VendorId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));
            #endregion

            #region Customer Mapping
            this.CreateMap<Customer, CustomerGridViewModel>()
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                 .ForMember(
                    dest => dest.Ext,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Ext))
                 .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(e => e.Default).Address.FullAddress));

            this.CreateBothMaps<Customer, CustomerCreateViewModel>();

            this.CreateMap<Customer, CustomerDetailsViewModel>()
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Phone))
                 .ForMember(
                    dest => dest.Ext,
                    opt => opt.MapFrom(src => src.Phones.FirstOrDefault(p => p.Default).Ext))
                 .ForMember(
                        dest => dest.FullAddress,
                        opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(e => e.Default).Address.FullAddress))
                 .ForMember(
                        dest => dest.StatusName,
                        opt => opt.MapFrom(src => src.StatusId.ToString()))
                  .ForMember(
                        dest => dest.CustomerSince,
                        opt => opt.MapFrom(src => src.CreatedDate.Date));

            this.CreateMap<Customer, CustomerUpdateViewModel>()
                .ForMember(dest => dest.GroupIds,
                            opt => opt.MapFrom(src => src.Groups.Select(ent => ent.CustomerGroupId)));

            this.CreateMap<CustomerUpdateViewModel, Customer>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.StatusId);
            #endregion

            #region Customer Phone Mapping
            this.CreateMap<CustomerPhone, EntityPhoneGridViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.CustomerId));
            this.CreateMap<CustomerPhone, EntityPhoneCreateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.CustomerId));
            this.CreateMap<EntityPhoneCreateViewModel, CustomerPhone>()
                .ForMember(
                        dest => dest.CustomerId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<CustomerPhone, EntityPhoneDetailsViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.CustomerId));
            this.CreateMap<CustomerPhone, EntityPhoneUpdateViewModel>()
                .ForMember(
                        dest => dest.EntityId,
                        opt => opt.MapFrom(src => src.CustomerId));
            this.CreateMap<EntityPhoneUpdateViewModel, CustomerPhone>()
                .ForMember(
                        dest => dest.CustomerId,
                        opt => opt.MapFrom(src => src.EntityId));
            #endregion

            #region Customer Address related mapping
            this.CreateMap<CustomerAddress, AddressBaseViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.CustomerId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<AddressCreateViewModel, CustomerAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.CustomerId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<AddressUpdateViewModel, CustomerAddress>()
                .ForMember(
                        dest => dest.AddressId,
                        opt => opt.MapFrom(src => src.AddressId))
                .ForMember(
                        dest => dest.CustomerId,
                        opt => opt.MapFrom(src => src.EntityId));

            this.CreateMap<CustomerAddress, AddressUpdateViewModel>()
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.CustomerId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));


            this.CreateMap<CustomerAddress, AddressDetailsViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.CustomerId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));

            this.CreateMap<CustomerAddress, AddressGridViewModel>()
                 .ForMember(
                     dest => dest.FullAddress,
                     opt => opt.MapFrom(src => src.Address.FullAddress))
                .ForMember(
                    dest => dest.EntityId,
                    opt => opt.MapFrom(src => src.CustomerId))
                 .ForMember(
                     dest => dest.AddressLine1,
                     opt => opt.MapFrom(src => src.Address.AddressLine1))
                 .ForMember(
                     dest => dest.AddressLine2,
                     opt => opt.MapFrom(src => src.Address.AddressLine2))
                 .ForMember(
                     dest => dest.City,
                     opt => opt.MapFrom(src => src.Address.City))
                 .ForMember(
                     dest => dest.State,
                     opt => opt.MapFrom(src => src.Address.State))
                 .ForMember(
                     dest => dest.ZipCode,
                     opt => opt.MapFrom(src => src.Address.ZipCode))
                 .ForMember(
                     dest => dest.CountryCode,
                     opt => opt.MapFrom(src => src.Address.CountryCode));
            #endregion

            #region CustomerContacts Mapping

            CreateMap<ContactCreateViewModel, CustomerContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.CustomerId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<MGCap.Domain.ViewModels.Contact.ContactAssignViewModel, CustomerContact>()
                .ForMember(dest => dest.CustomerId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<ContactUpdateViewModel, CustomerContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.CustomerId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<CustomerContact, ContactDetailsViewModel>()
                .ForMember(dest => dest.EntityId,
                            opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.ID,
                            opt => opt.MapFrom(src => src.ContactId))
                .ForMember(dest => dest.Guid,
                            opt => opt.MapFrom(src => src.Contact.Guid))
                .ForMember(dest => dest.FirstName,
                            opt => opt.MapFrom(src => src.Contact.FirstName))
                .ForMember(dest => dest.MiddleName,
                            opt => opt.MapFrom(src => src.Contact.MiddleName))
                .ForMember(dest => dest.LastName,
                            opt => opt.MapFrom(src => src.Contact.LastName))
                .ForMember(dest => dest.Salutation,
                            opt => opt.MapFrom(src => src.Contact.Salutation))
                .ForMember(dest => dest.Title,
                            opt => opt.MapFrom(src => src.Contact.Title))
                .ForMember(dest => dest.Notes,
                            opt => opt.MapFrom(src => src.Contact.Notes))
                .ForMember(dest => dest.Gender,
                            opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(dest => dest.DOB,
                            opt => opt.MapFrom(src => src.Contact.DOB))
                .ForMember(dest => dest.Email,
                            opt => opt.MapFrom(src => src.Contact.Emails.Select(e => e.Email).FirstOrDefault()))
                .ForMember(dest => dest.FullAddress,
                            opt => opt.MapFrom(src => src.Contact.Addresses.Select(e => e.Address).FirstOrDefault()))
                .ForMember(dest => dest.Phone,
                            opt => opt.MapFrom(src => src.Contact.Phones.Select(e => e.Phone).FirstOrDefault()));
            #endregion

            #region VendorContacts Mapping

            CreateMap<ContactCreateViewModel, VendorContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.VendorId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<MGCap.Domain.ViewModels.Contact.ContactAssignViewModel, VendorContact>()
                .ForMember(dest => dest.VendorId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<ContactUpdateViewModel, VendorContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.VendorId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<VendorContact, ContactDetailsViewModel>()
                .ForMember(dest => dest.EntityId,
                            opt => opt.MapFrom(src => src.VendorId))
                .ForMember(dest => dest.ID,
                            opt => opt.MapFrom(src => src.ContactId))
                .ForMember(dest => dest.Guid,
                            opt => opt.MapFrom(src => src.Contact.Guid))
                .ForMember(dest => dest.FirstName,
                            opt => opt.MapFrom(src => src.Contact.FirstName))
                .ForMember(dest => dest.MiddleName,
                            opt => opt.MapFrom(src => src.Contact.MiddleName))
                .ForMember(dest => dest.LastName,
                            opt => opt.MapFrom(src => src.Contact.LastName))
                .ForMember(dest => dest.Salutation,
                            opt => opt.MapFrom(src => src.Contact.Salutation))
                .ForMember(dest => dest.Title,
                            opt => opt.MapFrom(src => src.Contact.Title))
                .ForMember(dest => dest.Notes,
                            opt => opt.MapFrom(src => src.Contact.Notes))
                .ForMember(dest => dest.Gender,
                            opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(dest => dest.DOB,
                            opt => opt.MapFrom(src => src.Contact.DOB))
                .ForMember(dest => dest.Email,
                            opt => opt.MapFrom(src => src.Contact.Emails.Select(e => e.Email).FirstOrDefault()))
                .ForMember(dest => dest.FullAddress,
                            opt => opt.MapFrom(src => src.Contact.Addresses.Select(e => e.Address).FirstOrDefault()))
                .ForMember(dest => dest.Phone,
                            opt => opt.MapFrom(src => src.Contact.Phones.Select(e => e.Phone).FirstOrDefault()));
            #endregion

            #region Buildings Mapping
            CreateMap<Address, AddressViewModel>()
                .ForMember(dest => dest.AddressId,
                           opt => opt.MapFrom(src => src.ID));

            CreateMap<Building, BuildingUpdateViewModel>()
                .ForMember(dest => dest.Employees,
                            opt => opt.MapFrom(src => src.Employees))
                .ForMember(dest => dest.Address,
                           opt => opt.MapFrom(src => src.Address));

            CreateMap<BuildingEmployee, EmployeeBuildingViewModel>()
                .ForMember(dest => dest.ID,
                    opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Employee.Email))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Employee.Contact.FullName))
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Employee.Role.Name));

            CreateMap<EmployeeBuildingViewModel, BuildingEmployee>()
                .ForMember(dest => dest.EmployeeId,
                    opt => opt.MapFrom(src => src.ID));

            CreateMap<BuildingEmployee, EmployeeBuildingViewModel>()
                .ForMember(dest => dest.ID,
                    opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Employee.Email))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Employee.Contact.FullName))
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Employee.Role.Name));

            CreateMap<EmployeeBuildingViewModel, BuildingEmployee>()
                .ForMember(dest => dest.EmployeeId,
                    opt => opt.MapFrom(src => src.ID));

            CreateMap<BuildingCreateViewModel, Building>();
            CreateMap<BuildingUpdateViewModel, Building>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Employees);

            CreateMap<AddressViewModel, Address>();
            #endregion

            #region BuildingContacts Mapping

            CreateMap<ContactCreateViewModel, BuildingContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.BuildingId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<MGCap.Domain.ViewModels.Contact.ContactAssignViewModel, BuildingContact>()
                .ForMember(dest => dest.BuildingId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<ContactUpdateViewModel, BuildingContact>()
                .ForMember(dest => dest.ContactId,
                            opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.BuildingId,
                            opt => opt.MapFrom(src => src.EntityId));

            CreateMap<BuildingContact, ContactDetailsViewModel>()
                .ForMember(dest => dest.EntityId,
                            opt => opt.MapFrom(src => src.BuildingId))
                .ForMember(dest => dest.ID,
                            opt => opt.MapFrom(src => src.ContactId))
                .ForMember(dest => dest.Guid,
                            opt => opt.MapFrom(src => src.Contact.Guid))
                .ForMember(dest => dest.FirstName,
                            opt => opt.MapFrom(src => src.Contact.FirstName))
                .ForMember(dest => dest.MiddleName,
                            opt => opt.MapFrom(src => src.Contact.MiddleName))
                .ForMember(dest => dest.LastName,
                            opt => opt.MapFrom(src => src.Contact.LastName))
                .ForMember(dest => dest.Salutation,
                            opt => opt.MapFrom(src => src.Contact.Salutation))
                .ForMember(dest => dest.Title,
                            opt => opt.MapFrom(src => src.Contact.Title))
                .ForMember(dest => dest.Notes,
                            opt => opt.MapFrom(src => src.Contact.Notes))
                .ForMember(dest => dest.Gender,
                            opt => opt.MapFrom(src => src.Contact.Gender))
                .ForMember(dest => dest.DOB,
                            opt => opt.MapFrom(src => src.Contact.DOB))
                .ForMember(dest => dest.Email,
                            opt => opt.MapFrom(src => src.Contact.Emails.Select(e => e.Email).FirstOrDefault()))
                .ForMember(dest => dest.FullAddress,
                            opt => opt.MapFrom(src => src.Contact.Addresses.Select(e => e.Address).FirstOrDefault()))
                .ForMember(dest => dest.Phone,
                            opt => opt.MapFrom(src => src.Contact.Phones.Select(e => e.Phone).FirstOrDefault()));
            #endregion

            #region Department
            this.CreateMap<Department, DepartmentGridViewModel>();
            this.CreateMap<DepartmentCreateViewModel, Department>();
            this.CreateBothMaps<Department, DepartmentEditViewModel>();
            this.CreateMap<Department, DepartmentDetailsViewModel>();
            #endregion

            #region Work Orders
            CreateMap<ExternalWorkOrderViewModel, WorkOrder>()
                    .ForMember(
                        dest => dest.StatusId,
                        opt => opt.UseValue((int)WorkOrderStatus.Draft)
                    )
                    .ForMember(
                        dest => dest.SendRequesterNotifications,
                        opt => opt.UseValue(true)
                    );

            CreateMap<WorkOrderCreateViewModel, WorkOrder>()
                   .ForMember(
                        dest => dest.Notes,
                        opt => opt.MapFrom(src => src.Notes.Select(n => new WorkOrderNote
                        {
                            EmployeeId = n.EmployeeId,
                            Note = n.Note,
                            WorkOrderId = src.ID,
                        }))
                    )
                   .ForMember(
                        dest => dest.Tasks,
                        opt => opt.MapFrom(src => src.Tasks.Select(n => new WorkOrderTask
                        {
                            WorkOrderId = src.ID,
                            IsComplete = n.IsComplete,
                            Description = n.Description,
                            ServiceId = n.ServiceId,
                            UnitPrice = n.UnitPrice ?? 0,
                            Quantity = n.Quantity ?? 0,
                            DiscountPercentage = n.DiscountPercentage ?? 0,
                            LastCheckedDate = n.IsComplete ? DateTime.UtcNow : new DateTime(),
                            Note = n.Note,
                            Frequency = n.Frequency,
                            Location = n.Location,
                            Rate = n.Rate,
                            UnitFactor = n.UnitFactor,
                            WorkOrderServiceCategoryId = n.WorkOrderServiceCategoryId,
                            WorkOrderServiceId = n.WorkOrderServiceId,
                            HoursExecuted = n.HoursExecuted,
                            QuantityExecuted = n.QuantityExecuted,
                            QuantityRequiredAtClose = n.QuantityRequiredAtClose,
                            GeneralNote = n.GeneralNote
                        }))
                    ).
                    ForMember(
                        dest => dest.Attachments,
                        opt => opt.MapFrom(src => src.Attachments.Select(n => new WorkOrderAttachment
                        {
                            BlobName = n.BlobName,
                            FullUrl = n.FullUrl,
                            ImageTakenDate = n.ImageTakenDate,
                            Description = n.Description,
                            EmployeeId = n.EmployeeId,
                            WorkOrderId = n.WorkOrderId
                        }))
                    );

            CreateMap<WorkOrderUpdateViewModel, WorkOrder>()
                   .IgnoreMember(dest => dest.CompanyId)
                   .IgnoreMember(dest => dest.Guid)
                   .IgnoreMember(dest => dest.Number);

            CreateMap<WorkOrder, WorkOrderUpdateViewModel>()
                   .ForMember(
                        dest => dest.BuildingName,
                        opt => opt.MapFrom(src => src.Building.Name)
                    )
                   .ForMember(
                        dest => dest.Notes,
                        opt => opt.MapFrom(src => Mapper.Map<ICollection<WorkOrderNote>, WorkOrderNoteUpdateViewModel[]>(src.Notes))
                    )
                   .ForMember(
                        dest => dest.Tasks,
                        opt => opt.MapFrom(src => Mapper.Map<ICollection<WorkOrderTask>, WorkOrderTaskUpdateViewModel[]>(src.Tasks))
                    )
                   .ForMember(
                        dest => dest.Attachments,
                        opt => opt.MapFrom(src => Mapper.Map<ICollection<WorkOrderAttachment>, WorkOrderAttachmentUpdateViewModel[]>(src.Attachments))
                    );

            CreateMap<WorkOrderUpdateViewModel, WorkOrderCreateViewModel>()
                .IgnoreMember(dest => dest.Attachments)
                .IgnoreMember(dest => dest.Notes)
                .IgnoreMember(dest => dest.Tasks)
                .IgnoreMember(dest => dest.ID);

            CreateMap<WorkOrderTaskSummaryViewModel, WorkOrder>()
                .ForMember(
                    dest => dest.Tasks,
                    opt => opt.MapFrom(src => src.Tasks.Select(n => new WorkOrderTask()
                    {
                        WorkOrderId = src.ID,
                        IsComplete = n.IsComplete,
                        Description = n.Description,
                        ServiceId = n.ServiceId,
                        UnitPrice = n.UnitPrice ?? 0,
                        Quantity = n.Quantity ?? 0,
                        DiscountPercentage = n.DiscountPercentage ?? 0,
                        LastCheckedDate = n.IsComplete ? DateTime.UtcNow : new DateTime(),
                        Note = n.Note
                    }))
                );

            #endregion

            #region Work Order Notes
            CreateMap<WorkOrderNote, WorkOrderNote>();

            CreateMap<WorkOrderNoteCreateViewModel, WorkOrderNote>();

            CreateMap<WorkOrderNote, WorkOrderNoteUpdateViewModel>()
                    .ForMember(
                        dest => dest.EmployeeFullName,
                        opt => opt.MapFrom(src => src.Employee.Contact.FullName)
                    );

            CreateMap<WorkOrderNoteUpdateViewModel, WorkOrderNote>();
            #endregion

            #region Work Order Tasks

            CreateMap<WorkOrderTask, WorkOrderTask>();

            CreateMap<WorkOrderTaskCreateViewModel, WorkOrderTask>()
                .ForMember(dest => dest.LastCheckedDate,
                           opt => opt.MapFrom(src => src.IsComplete ? DateTime.UtcNow : new DateTime()));

            CreateMap<WorkOrderTaskUpdateViewModel, WorkOrderTask>();

            CreateMap<WorkOrderTask, WorkOrderTaskUpdateViewModel>()
                .ForMember(dest => dest.ServiceName,
                           opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.UnitFactor,
                           opt => opt.MapFrom(src => src.Service.UnitFactor));

            CreateMap<WorkOrderTask, WorkOrderTaskDetailsViewModel>()
                .ForMember(dest => dest.ServiceName,
                           opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.UnitFactor,
                           opt => opt.MapFrom(src => src.Service.UnitFactor));

            CreateBothMaps<WorkOrderTaskUpdateViewModel, WorkOrderTaskCreateViewModel>();

            CreateBothMaps<WorkOrderTaskAttachment, WorkOrderTaskCreateViewModel>();

            CreateBothMaps<WorkOrderTaskAttachmentBaseViewModel, WorkOrderTaskAttachment>();

            CreateBothMaps<WorkOrderTaskAttachmentCreateViewModel, WorkOrderTaskAttachment>();
            #endregion

            #region Work Order Attachments

            CreateMap<WorkOrderAttachmentCreateViewModel, WorkOrderAttachment>();

            CreateMap<WorkOrderAttachment, WorkOrderAttachmentUpdateViewModel>();

            CreateMap<WorkOrderAttachmentUpdateViewModel, WorkOrderAttachment>();

            CreateMap<WorkOrderAttachmentBaseViewModel, WorkOrderAttachment>();

            #endregion Work Order Attachments

            #region Contracts
            this.CreateBothMaps<Contract, ContractCreateViewModel>();
            this.CreateBothMaps<Contract, ContractUpdateViewModel>();
            this.CreateBothMaps<Contract, ContractGridViewModel>();
            #endregion

            #region Services
            this.CreateBothMaps<Service, ServiceCreateViewModel>();
            this.CreateBothMaps<Service, ServiceGridViewModel>();
            this.CreateBothMaps<Service, ServiceUpdateViewModel>();
            #endregion

            #region CleaningReport
            this.CreateMap<CleaningReportCreateViewModel, CleaningReport>();
            this.CreateMap<CleaningReport, CleaningReportCreateViewModel>();

            this.CreateMap<CleaningReportUpdateViewModel, CleaningReport>()
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.Submitted)
                .IgnoreMember(dest => dest.Number);

            this.CreateMap<CleaningReport, CleaningReportUpdateViewModel>();
            #endregion

            #region CLeaningReportItem
            this.CreateMap<CleaningReportItem, CleaningReportItemCreateViewModel>();

            this.CreateMap<CleaningReportItemCreateViewModel, CleaningReportItem>()
                                    .ForMember(
                        dest => dest.CleaningReportItemAttachments,
                        opt => opt.MapFrom(src => src.Attachments.Select(n => new CleaningReportItemAttachment
                        {
                            BlobName = n.BlobName,
                            FullUrl = n.FullUrl,
                            ImageTakenDate = n.ImageTakenDate,
                            Title = n.Title,
                            CleaningReportItemId = n.CleaningReportItemId
                        }))
                    );

            this.CreateMap<CleaningReportItem, CleaningReportItemUpdateViewModel>()
                   .ForMember(
                        dest => dest.Attachments,
                        opt => opt.MapFrom(src => Mapper.Map<ICollection<CleaningReportItemAttachment>, CleaningReportItemAttachmentUpdateViewModel[]>(src.CleaningReportItemAttachments))
                    );

            this.CreateMap<CleaningReportItemUpdateViewModel, CleaningReportItem>();

            #endregion

            #region CleaningReportItemAttachment
            CreateMap<CleaningReportItemAttachmentCreateViewModel, CleaningReportItemAttachment>();

            CreateMap<CleaningReportItemAttachment, CleaningReportItemAttachmentUpdateViewModel>();

            CreateMap<CleaningReportItemAttachmentUpdateViewModel, CleaningReportItemAttachment>();
            #endregion

            #region Common ListBox
            this.CreateMap<Department, ListBoxViewModel>();

            this.CreateMap<CustomerGroup, ListBoxViewModel>();
            this.CreateMap<CustomerCustomerGroup, ListBoxViewModel>()
                        .ForMember(dest => dest.ID,
                                    opt => opt.MapFrom(src => src.CustomerGroup.ID))
                        .ForMember(dest => dest.Name,
                                    opt => opt.MapFrom(src => src.CustomerGroup.Name));

            this.CreateMap<VendorGroup, ListBoxViewModel>();
            this.CreateMap<VendorVendorGroup, ListBoxViewModel>()
                        .ForMember(dest => dest.ID,
                                    opt => opt.MapFrom(src => src.VendorGroup.ID))
                        .ForMember(dest => dest.Name,
                                    opt => opt.MapFrom(src => src.VendorGroup.Name));


            this.CreateMap<Vendor, ListBoxViewModel>();
            #endregion

            #region CustomerUser

            CreateMap<CustomerUserSignUpViewModel, CustomerUser>();

            #endregion

            #region Ticket
            this.CreateMap<TicketCreateViewModel, Ticket>()
                .ForMember(
                    dest => dest.Attachments,
                    opt => opt.MapFrom(src => src.Attachments.Select(n => new TicketAttachment
                    {
                        BlobName = n.BlobName,
                        FullUrl = n.FullUrl,
                        Description = n.Description,
                        TicketId = src.ID
                    }))
                )
                .ForMember(
                    dest => dest.Data,
                    opt => opt.MapFrom(src => DictionaryUtils.ToDictionary<string>(src.Data)
                ));

            this.CreateMap<TicketUpdateViewModel, Ticket>()
                .IgnoreMember(dest => dest.CompanyId)
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.Number)
                .ForMember(
                    dest => dest.Data,
                    opt => opt.MapFrom(src => DictionaryUtils.ToDictionary<string>(src.Data)
                ));

            this.CreateMap<Ticket, TicketUpdateViewModel>()
                .ForMember(
                    dest => dest.Attachments,
                    opt => opt.MapFrom(src => Mapper.Map<ICollection<TicketAttachment>, TicketAttachmentUpdateViewModel[]>(src.Attachments)
                ))
                .ForMember(
                    dest => dest.Data,
                    opt => opt.MapFrom(src => DictionaryUtils.DictionaryToObject<TicketAdditionalData>(src.Data)
                ));
            #endregion

            #region Ticket Attachments
            CreateMap<TicketAttachmentCreateViewModel, TicketAttachment>();

            CreateBothMaps<TicketAttachmentUpdateViewModel, TicketAttachment>();
            #endregion Ticket Attachments


            #region Mobile App Version

            CreateBothMaps<MobileAppVersion, MobileAppVersionViewModel>();

            #endregion

            #region Cleaning Report Note

            CreateMap<CleaningReportNoteCreateViewModel, CleaningReportNote>();

            #endregion

            #region Office Type
            CreateBothMaps<OfficeServiceType, OfficeServiceTypeBaseViewModel>();
            CreateBothMaps<OfficeServiceType, OfficeServiceTypeGridViewModel>();
            CreateBothMaps<OfficeServiceType, OfficeServiceTypeListViewModel>();
            CreateBothMaps<OfficeServiceType, OfficeServiceTypeUpdateViewModel>();
            CreateBothMaps<OfficeServiceType, OfficeServiceTypeDetailsViewModel>();
            #endregion

            #region ContractItems
            CreateBothMaps<ContractItem, ContractItemBaseViewModel>();
            CreateBothMaps<ContractItem, ContractItemCreateViewModel>();
            CreateBothMaps<ContractItem, ContractItemUpdateViewModel>();
            CreateBothMaps<ContractItem, ContractItemGridViewModel>();
            #endregion

            #region Expense Types
            CreateBothMaps<ExpenseType, ExpenseTypeCreateViewModel>();
            CreateBothMaps<ExpenseType, ExpenseTypeDetailsViewModel>();
            CreateBothMaps<ExpenseType, ExpenseTypeUpdateViewModel>();
            CreateBothMaps<ExpenseType, ExpenseTypeBaseViewModel>();
            CreateBothMaps<ExpenseType, ExpenseTypeGridViewModel>();
            CreateBothMaps<ExpenseType, ExpenseTypeListBoxViewModel>();
            #endregion

            #region Expense Subcategories
            CreateBothMaps<ExpenseSubcategory, ExpenseSubcategoryBaseViewModel>();
            CreateBothMaps<ExpenseSubcategory, ExpenseSubcategoryCreateViewModel>();
            CreateBothMaps<ExpenseSubcategory, ExpenseSubcategoryUpdateViewModel>();
            CreateBothMaps<ExpenseSubcategory, ExpenseSubcategoryListBoxViewModel>();
            CreateBothMaps<ExpenseSubcategory, ExpenseSubcategoryDetailsViewModel>();
            #endregion

            #region Contract Expenses
            CreateBothMaps<ContractExpense, ContractExpenseBaseViewModel>();
            CreateBothMaps<ContractExpense, ContractExpenseCreateViewModel>();
            CreateBothMaps<ContractExpense, ContractExpenseUpdateViewModel>();
            CreateBothMaps<ContractExpense, ContractExpenseGridViewModel>();
            #endregion

            #region Proposals
            CreateBothMaps<Proposal, ProposalBaseViewModel>();
            CreateBothMaps<Proposal, ProposalCreateViewModel>();
            CreateBothMaps<Proposal, ProposalDetailViewModel>();
            CreateBothMaps<Proposal, ProposalGridViewModel>();
            CreateBothMaps<Proposal, ProposalReportDetailViewModel>();
            CreateBothMaps<Proposal, ProposalUpdateViewModel>();
            #endregion

            #region Inspection
            CreateBothMaps<Inspection, InspectionBaseViewModel>();
            CreateBothMaps<Inspection, InspectionCreateViewModel>();
            CreateBothMaps<Inspection, InspectionDetailViewModel>();
            CreateBothMaps<Inspection, InspectionGridViewModel>();
            CreateBothMaps<Inspection, InspectionUpdateViewModel>();
            CreateBothMaps<Inspection, InspectionReportDetailViewModel>();

            this.CreateMap<InspectionCreateViewModel, Inspection>();
            this.CreateMap<Inspection, InspectionCreateViewModel>();

            this.CreateMap<InspectionUpdateViewModel, CleaningReport>()
                .IgnoreMember(dest => dest.Guid)
                .IgnoreMember(dest => dest.Submitted)
                .IgnoreMember(dest => dest.Number);

            CreateMap<InspectionItemAttachment, InspectionItemAttachmentUpdateViewModel>();

            CreateMap<InspectionItemAttachmentUpdateViewModel, InspectionItemAttachment>();
            #endregion

            #region Inspection item
            CreateBothMaps<InspectionItem, InspectionItemBaseViewModel>();
            CreateBothMaps<InspectionItem, InspectionItemCreateViewModel>();
            CreateBothMaps<InspectionItem, InspectionReportDetailViewModel>();
            CreateBothMaps<InspectionItem, InspectionItemGridViewModel>();
            CreateBothMaps<InspectionItem, InspectionItemUpdateViewModel>();

            this.CreateMap<InspectionItemCreateViewModel, InspectionItem>()
                                        .ForMember(
                            dest => dest.Attachments,
                            opt => opt.MapFrom(src => src.Attachments.Select(n => new InspectionItemAttachment
                            {
                                BlobName = n.BlobName,
                                FullUrl = n.FullUrl,
                                ImageTakenDate = n.ImageTakenDate,
                                Title = n.Title,
                                Description = n.Description,
                                InspectionItemId = n.InspectionItemId
                            }))
                        ).ForMember(
                            dest => dest.Tasks,
                            opt => opt.MapFrom(src => src.Tasks.Select(n => new InspectionItemTask
                            {
                                InspectionItemId = src.ID,
                                IsComplete = n.IsComplete,
                                Description = n.Description
                            }))
                        ).ForMember(
                        dest => dest.Notes,
                        opt => opt.MapFrom(src => src.Notes.Select(n => new InspectionItemNote
                        {
                            EmployeeId = n.EmployeeId,
                            Note = n.Note,
                            InspectionItemId = src.InspectionId,
                        }))
                    );

            this.CreateMap<InspectionItem, InspectionItemUpdateViewModel>()
                .ForMember(
                    dest => dest.Attachments,
                    opt => opt.MapFrom(src => Mapper.Map<ICollection<InspectionItemAttachment>, InspectionItemAttachmentUpdateViewModel[]>(src.Attachments))
                ).ForMember(
                    dest => dest.Tasks,
                    opt => opt.MapFrom(src => Mapper.Map<ICollection<InspectionItemTask>, InspectionItemTaskUpdateViewModel[]>(src.Tasks))
                ).ForMember(
                    dest => dest.Notes,
                    opt => opt.MapFrom(src => Mapper.Map<ICollection<InspectionItemNote>, InspectionItemNoteUpdateViewModel[]>(src.Notes))
                );

            this.CreateMap<InspectionItemUpdateViewModel, InspectionItem>();
            #endregion

            #region Inspection note
            CreateBothMaps<InspectionNote, InspectionNoteBaseViewModel>();
            CreateBothMaps<InspectionNote, InspectionNoteCreateViewModel>();
            CreateBothMaps<InspectionNote, InspectionNoteUpdateViewModel>();

            this.CreateMap<InspectionNoteCreateViewModel, Inspection>();
            this.CreateMap<Inspection, InspectionNoteCreateViewModel>();
            #endregion

            #region Inspection Item Ticket
            CreateBothMaps<InspectionItemTicket, InspectionItemTicketBaseViewModel>();
            CreateBothMaps<InspectionItemTicket, InspectionItemTicketCreateViewModel>();
            #endregion

            #region inspectionAttachment
            CreateMap<InspectionItemAttachmentBaseViewModel, InspectionItemAttachment>();
            CreateMap<InspectionItemAttachment, InspectionItemAttachmentBaseViewModel>();
            CreateMap<InspectionItemAttachmentBaseViewModel, InspectionItemAttachment>();

            CreateMap<InspectionItemAttachmentCreateViewModel, InspectionItemAttachment>();

            CreateMap<InspectionItemAttachment, InspectionItemAttachmentUpdateViewModel>();

            CreateMap<InspectionItemAttachmentUpdateViewModel, InspectionItemAttachment>();

            #endregion

            #region Proposal Services
            CreateBothMaps<ProposalService, ProposalServiceBaseViewModel>();
            CreateBothMaps<ProposalService, ProposalServiceCreateViewModel>();
            CreateBothMaps<ProposalService, ProposalServiceDetailViewModel>();
            CreateBothMaps<ProposalService, ProposalServiceGridViewModel>();
            CreateBothMaps<ProposalService, ProposalServiceUpdateViewModel>();
            #endregion

            #region Expense
            CreateBothMaps<Expense, ExpenseBaseViewModel>();
            CreateBothMaps<Expense, ExpenseCreateViewModel>();
            CreateBothMaps<Expense, ExpenseUpdateViewModel>();
            CreateBothMaps<Expense, ExpenseGridViewModel>();
            #endregion

            #region inspectionItemTask
            CreateMap<InspectionItemTaskBaseViewModel, InspectionItemTask>();
            CreateMap<InspectionItemTask, InspectionItemTaskBaseViewModel>();

            CreateMap<InspectionItemTaskCreateViewModel, InspectionItemTask>();
            CreateMap<InspectionItemTask, InspectionItemTaskUpdateViewModel>();
            CreateMap<InspectionItemTaskUpdateViewModel, InspectionItemTask>();
            #endregion

            #region Contract Office Space
            CreateBothMaps<ContractOfficeSpace, ContractOfficeSpaceBaseViewModel>();
            CreateBothMaps<ContractOfficeSpace, ContractOfficeSpaceCreateViewModel>();
            CreateBothMaps<ContractOfficeSpace, ContractOfficeSpaceUpdateViewModel>();
            CreateBothMaps<ContractOfficeSpace, ContractOfficeSpaceGridViewModel>();
            #endregion

            #region Revenue
            CreateBothMaps<Revenue, RevenueBaseViewModel>();
            CreateBothMaps<Revenue, RevenueCreateViewModel>();
            CreateBothMaps<Revenue, RevenueUpdateViewModel>();
            CreateBothMaps<Revenue, RevenueGridViewModel>();
            #endregion

            #region PreCalendar
            CreateBothMaps<PreCalendar, PreCalendarBaseViewModel>();
            CreateBothMaps<PreCalendar, PreCalendarGridViewModel>();
            CreateBothMaps<PreCalendar, PreCalendarUpdateViewModel>();

            CreateMap<PreCalendarCreateViewModel, PreCalendar>()
                .ForMember(
                    dest => dest.Tasks,
                        opt => opt.MapFrom(src => src.Tasks.Select(n => new PreCalendarTask
                        {
                            PreCalendarId = src.ID,
                            IsComplete = n.IsComplete,
                            Description = n.Description,
                            ServiceId = n.ServiceId,
                            UnitPrice = n.UnitPrice ?? 0,
                            Quantity = n.Quantity ?? 0,
                            DiscountPercentage = n.DiscountPercentage ?? 0,
                            LastCheckedDate = n.IsComplete ? DateTime.UtcNow : new DateTime(),
                            Note = n.Note
                        }))
                );

            CreateMap<PreCalendar, PreCalendarCreateViewModel>()
               .ForMember(
                    dest => dest.Tasks,
                    opt => opt.MapFrom(src => Mapper.Map<ICollection<PreCalendarTask>, PreCalendarCreateViewModel[]>(src.Tasks))
                );

            CreateMap<PreCalendarUpdateViewModel, PreCalendar>()
               .IgnoreMember(dest => dest.CompanyId)
               .IgnoreMember(dest => dest.Guid);

            CreateMap<PreCalendar, PreCalendarUpdateViewModel>()
                   .ForMember(
                        dest => dest.Tasks,
                        opt => opt.MapFrom(src => Mapper.Map<ICollection<PreCalendarTask>, PreCalendarTaskUpdateViewModel[]>(src.Tasks))
                    );
            #endregion

            #region PreCalendar Task
            CreateMap<PreCalendarTaskCreateViewModel, PreCalendarTask>()
                .ForMember(dest => dest.LastCheckedDate,
                    opt => opt.MapFrom(src => src.IsComplete ? DateTime.UtcNow : new DateTime()));

            CreateMap<PreCalendarTaskUpdateViewModel, PreCalendarTask>();

            CreateMap<PreCalendarTask, PreCalendarTaskUpdateViewModel>()
                .ForMember(dest => dest.ServiceName,
                           opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.UnitFactor,
                           opt => opt.MapFrom(src => src.Service.UnitFactor));
            #endregion

            #region inspection Item Notes
            CreateMap<InspectionItemNoteBaseViewModel, InspectionItemNote>();
            CreateMap<InspectionItemNote, InspectionItemNoteBaseViewModel>();
            CreateMap<InspectionItemNoteCreateViewModel, InspectionItemNote>();
            CreateMap<InspectionItemNoteUpdateViewModel, InspectionItemNote>();
            #endregion

            #region Company Setings
            CreateMap<CompanySettingsBaseViewModel, CompanySettings>();
            CreateMap<CompanySettingsUpdateViewModel, CompanySettings>();
            CreateMap<CompanySettingsDetailViewModel, CompanySettings>();
            #endregion

            #region Roles
            this.CreateBothMaps<RoleBaseViewModel, Role>();
            this.CreateBothMaps<RoleCreateViewModel, Role>();
            this.CreateBothMaps<RoleUpdateViewModel, Role>();
            #endregion

            #region Expense Types
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryCreateViewModel>();
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryDetailsViewModel>();
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryUpdateViewModel>();
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryBaseViewModel>();
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryGridViewModel>();
            CreateBothMaps<ScheduleSettingCategory, ScheduleCategoryListBoxViewModel>();
            #endregion

            #region Expense Subcategories
            CreateBothMaps<ScheduleSettingSubCategory, ScheduleSubCategoryBaseViewModel>();
            CreateBothMaps<ScheduleSettingSubCategory, ScheduleSubCategoryCreateViewModel>();
            CreateBothMaps<ScheduleSettingSubCategory, ScheduleSubCategoryUpdateViewModel>();
            CreateBothMaps<ScheduleSettingSubCategory, ScheduleSubCategoryListBoxViewModel>();
            CreateBothMaps<ScheduleSettingSubCategory, ScheduleSubCategoryDetailsViewModel>();
            #endregion

            #region Calendar Item Frequency
            CreateBothMaps<CalendarItemFrequency, CalendarItemFrequencyCreateViewModel>();
            #endregion

            #region Contract Note
            this.CreateBothMaps<ContractNote, ContractNoteCreateViewModel>();
            #endregion

            #region Contract Activity Log Note
            this.CreateBothMaps<ContractActivityLogNote, ContractActivityLogNoteCreateViewModel>();
            this.CreateBothMaps<ContractActivityLogNote, ContractActivityLogNoteUpdateViewModel>();
            #endregion

            #region Work Order Schedule Setting
            this.CreateBothMaps<WorkOrderScheduleSetting, WorkOrderScheduleSettingBaseViewModel>();
            this.CreateBothMaps<WorkOrderScheduleSetting, WorkOrderScheduleSettingCreateViewModel>();
            this.CreateBothMaps<WorkOrderScheduleSetting, WorkOrderScheduleSettingUpdateViewModel>();
            #endregion

            #region Tags
            this.CreateBothMaps<Tag, TagBaseViewModel>();
            this.CreateBothMaps<Tag, TagCreateViewModel>();
            this.CreateBothMaps<Tag, TagUpdateViewModel>();

            this.CreateBothMaps<TicketTag, TicketTagBaseViewModel>();
            this.CreateBothMaps<TicketTag, TicketTagCreateViewModel>();
            this.CreateBothMaps<TicketTag, TicketTagDeleteViewModel>();
            #endregion

            #region Work Order Service Category
            CreateBothMaps<WorkOrderServiceCategory, WorkOrderServiceCategoryBaseViewModel>();
            CreateBothMaps<WorkOrderServiceCategory, WorkOrderServiceCategoryCreateViewModel>();
            CreateBothMaps<WorkOrderServiceCategory, WorkOrderServiceCategoryUpdateViewModel>();
            #endregion

            #region Work Order Service
            CreateBothMaps<WorkOrderService, WorkOrderServiceBaseViewModel>();
            CreateBothMaps<WorkOrderService, WorkOrderServiceCreateViewModel>();
            CreateBothMaps<WorkOrderService, WorkOrderServiceUpdateViewModel>();
            #endregion
        }

        /// <summary>
        ///      Wrapper of CreateMap<TSource, TDestination>
        ///      performs mapping in both ways
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TDestination">Destination Type</typeparam>
        public void CreateBothMaps<TSource, TDestination>()
        {
            this.CreateMap<TSource, TDestination>();
            this.CreateMap<TDestination, TSource>();
        }
    }
}
