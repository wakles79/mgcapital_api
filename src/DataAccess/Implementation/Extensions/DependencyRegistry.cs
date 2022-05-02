// -----------------------------------------------------------------------
// <copyright file="RepositoryCollectionExtensions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Extensions
{
    /// <summary>
    ///     Configure the dependencies of the DataAccess layer.
    /// </summary>
    public static class DependencyRegistry
    {
        /// <summary>
        ///     Through the <see cref="IServiceCollection"/> configure
        ///     the dependencies for injecting the actual implementation
        ///     of the interfaces
        /// </summary>
        /// <param name="services">Service for configuring the dependencies</param>
        /// <returns>The <paramref name="services"/> after configuring the contracts</returns>
        public static IServiceCollection ConfigureDataAccessDependencies(this IServiceCollection services)
        {
            services.AddTransient<IBaseDapperRepository, BaseDapperRepository>();

            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IContactsRepository, ContactsRepository>();
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IContactPhonesRepository, ContactPhonesRepository>();
            services.AddScoped<IContactEmailsRepository, ContactEmailsRepository>();
            services.AddScoped<IAddressesRepository, AddressesRepository>();
            services.AddScoped<IContactAddressesRepository, ContactAddressesRepository>();
            services.AddScoped<IVendorsRepository, VendorsRepository>();
            services.AddScoped<IVendorPhonesRepository, VendorPhonesRepository>();
            services.AddScoped<IVendorEmailsRepository, VendorEmailsRepository>();
            services.AddScoped<IVendorAddressesRepository, VendorAddressesRepository>();
            services.AddScoped<ICustomersRepository, CustomersRepository>();
            services.AddScoped<ICustomerPhonesRepository, CustomerPhonesRepository>();
            services.AddScoped<ICustomerAddressesRepository, CustomerAddressesRepository>();
            services.AddScoped<IVendorContactsRepository, VendorContactsRepository>();
            services.AddScoped<IBuildingContactsRepository, BuildingContactsRepository>();
            services.AddScoped<ICustomerContactsRepository, CustomerContactsRepository>();
            services.AddScoped<ICustomerGroupsRepository, CustomerGroupsRepository>();
            services.AddScoped<ICustomerCustomerGroupsRepository, CustomerCustomerGroupsRepository>();
            services.AddScoped<IVendorGroupsRepository, VendorGroupsRepository>();
            services.AddScoped<IVendorVendorGroupsRepository, VendorVendorGroupsRepository>();
            services.AddScoped<ICustomerEmployeesRepository, CustomerEmployeesRepository>();
            services.AddScoped<IBuildingsRepository, BuildingsRepository>();
            services.AddScoped<IWorkOrdersRepository, WorkOrdersRepository>();
            services.AddScoped<IWorkOrderNotesRepository, WorkOrderNotesRepository>();
            services.AddScoped<IWorkOrderTasksRepository, WorkOrderTasksRepository>();
            services.AddScoped<IWorkOrderAttachmentRepository, WorkOrderAttachmentRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<IContractsRepository, ContractsRepository>();
            services.AddScoped<IWorkOrderStatusLogRepository, WorkOrderStatusLogRepository>();
            services.AddScoped<IWorkOrderActivityLogRepository, WorkOrderActivityLogRepository>();
            services.AddScoped<IWorkOrderNotificationTemplatesRepository, WorkOrderNotificationTemplatesRepository>();
            //services.AddScoped<ITicketNotificationTemplatesRepository, TicketNotificationTemplatesRepository>(); mg-15
            services.AddScoped<IWorkOrderEmployeesRepository, WorkOrderEmployeesRepository>();

            services.AddScoped<IBuildingEmployeesRepository, BuildingEmployeesRepository>();

            services.AddScoped<IDirectoryRepository, DirectoryRepository>();
            services.AddScoped<ICleaningReportsRepository, CleaningReportsRepository>();
            services.AddScoped<ICleaningReportItemsRepository, CleaningReportItemsRepository>();
            services.AddScoped<ICleaningReportItemAttachmentsRepository, CleaningReportItemAttachmentsRepository>();
            services.AddScoped<IPushNotificationRepository, PushNotificationRepository>();
            services.AddScoped<ICustomerUserRepository, CustomerUserRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();
            services.AddScoped<ITicketAttachmentRepository, TicketAttachmentRepository>();

            services.AddScoped<ICustomerFeedRepository, CustomerFeedRepository>();
            services.AddScoped<IMobileAppVersionRepository, MobileAppVersionRepository>();
            services.AddScoped<ICleaningReportNoteRepository, CleaningReportNoteRepository>();

            services.AddScoped<IOfficeServiceTypesRepository, OfficeServiceTypesRepository>();
            services.AddScoped<IContractItemsRepository, ContractItemsRepository>();

            services.AddScoped<IExpenseTypesRepository, ExpenseTypesRepository>();
            services.AddScoped<IExpenseSubcategoriesRepository, ExpenseSubcategoriesRepository>();

            services.AddScoped<IContractExpensesRepository, ContractExpensesRepository>();

            services.AddScoped<IProposalsRepository, ProposalsRepository>();

            services.AddScoped<IProposalServicesRepository, ProposalServicesRepository>();

            services.AddScoped<ICleaningReportActivityLogRepository, CleaningReportActivityLogRepository>();

            services.AddScoped<IEmailActivityLogRepository, EmailActivityLogRepository>();

            services.AddScoped<IInspectionsRepository, InspectionsRepository>();

            services.AddScoped<IInspectionItemsRepository, InspectionItemsRepository>();

            services.AddScoped<IInspectionItemAttachmentsRepository, InspectionItemAttachmentsRepository>();

            services.AddScoped<IExpensesRepository, ExpensesRepository>();

            services.AddScoped<IInspectionActivityLogRepository, InspectionActivityLogRepository>();

            services.AddScoped<IInspectionItemTicketsRepository, InspectionItemTicketRepository>();

            services.AddScoped<IInspectionItemTasksRepository, InspectionItemTasksRepository>();

            services.AddScoped<IContractOfficeSpacesRepository, ContractOfficeSpacesRepository>();

            services.AddScoped<IRevenuesRepository, RevenuesRepository>();

            services.AddScoped<IPreCalendarRepository, PreCalendarRepository>();

            services.AddScoped<IInspectionNotesRepository, InspectionNotesRepository>();

            services.AddScoped<IInspectionItemNotesRepository, InspectionItemNotesRepository>();


            services.AddScoped<IPreCalendarTasksRepository, PreCalendarTasksRepository>();
            services.AddScoped<ICompanySettingsRepository, CompanySettingsRepository>();

            services.AddScoped<IContractActivityLogRepository, ContractActivityLogRepository>();

            services.AddScoped<IBuildingActivityLogRepository, BuildingActivityLogRepository>();

            services.AddScoped<IRolesRepository, RolesRepository>();

            services.AddScoped<IPermissionsRepository, PermissionsRepository>();

            services.AddScoped<IPermissionRolesRepository, PermissionRolesRepository>();

            services.AddScoped<IScheduleCategoriesRepository, ScheduleCategoriesRepository>();

            services.AddScoped<IScheduleSubCategoriesRepository, ScheduleSubCategoriesRepository>();

            services.AddScoped<ICalendarItemFrequenciesRepository, CalendarItemFrequenciesRepository>();

            services.AddScoped<IContractNoteRepository, ContractNoteRepository>();

            services.AddScoped<IContractActivityLogNotesRepository, ContractActivityLogNotesRepository>();

            services.AddScoped<IWorkOrderScheduleSettingsRepository, WorkOrderScheduleSettingsRepository>();

            services.AddScoped<ITicketActivityLogRepository, TicketActivityLogRepository>();

            services.AddScoped<ITicketActivityLogStatusRepository, TicketActivityLogStatusRepository>();

            services.AddScoped<ITicketCategoriesRepository, TicketCategoriesRepository>();

            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITicketTagRepository, TicketTagRepository>();

            services.AddScoped<IWorkOrderTaskAttachmentsRepository, WorkOrderTaskAttachmentsRepository>();

            services.AddScoped<IWorkOrderServiceCategoriesRepository, WorkOrderServiceCategoriesRepository>();
            services.AddScoped<IWorkOrderServicesRepository, WorkOrderServicesRepository>();

            services.AddScoped<IConvertedTicketsRepository, ConvertedTicketsRepository>();

            services.AddScoped<ITicketEmailHistoryRepository, TicketEmailHistoryRepository>();

            return services;
        }
    }
}
