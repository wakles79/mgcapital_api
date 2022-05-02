// -----------------------------------------------------------------------
// <copyright file="DependencyRegistry.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Business.Implementation.ApplicationServices;
using Microsoft.Extensions.DependencyInjection;

namespace MGCap.Business.Implementation.Extensions
{
    /// <summary>
    ///     Configure the dependencies of the Business layer.
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
        public static IServiceCollection ConfigureBusinessDependencies(this IServiceCollection services)
        {
            // Email and SMSs app services
            services.AddTransient<IEmailSender, MessageSender>();
            services.AddTransient<ISmsSender, MessageSender>();

            services.AddTransient<IToolsApplicationService, ToolsApplicationService>();

            services.AddScoped<IAddressesApplicationService, AddressesApplicationService>();
            services.AddScoped<IEmployeesApplicationService, EmployeesApplicationService>();
            services.AddScoped<IContactsApplicationService, ContactsApplicationService>();
            services.AddScoped<IDepartmentsApplicationService, DepartmentsApplicationService>();
            services.AddScoped<IVendorsApplicationService, VendorsApplicationService>();
            services.AddScoped<ICustomersApplicationService, CustomersApplicationService>();
            services.AddScoped<IBuildingsApplicationService, BuildingsApplicationService>();
            services.AddScoped<IWorkOrdersApplicationService, WorkOrdersApplicationService>();
            services.AddScoped<ITicketsApplicationService, TicketsApplicationService>();
            services.AddScoped<IWorkOrderFromMailApplicationService, WorkOrderFromMailApplicationService>();
            services.AddScoped<IServicesApplicationService, ServicesApplicationService>();
            services.AddScoped<IContractsApplicationService, ContractsApplicationService>();
            services.AddScoped<IWorkOrderNotificationsApplicationService, WorkOrderNotificationsApplicationService>();
            //services.AddScoped<ITicketNotificationApplicationService, TicketNotificationApplicationService>();
            services.AddScoped<IDirectoryApplicationService, DirectoryApplicationService>();
            services.AddScoped<IWorkOrderActivityLogApplicationService, WorkOrderActivityLogApplicationService>();
            services.AddScoped<IWorkOrderStatusLogApplicationService, WorkOrderStatusLogApplicationService>();
            services.AddScoped<ICleaningReportsApplicationService, CleaningReportsApplicationService>();

            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<ICustomerUserService, CustomerUserService>();
            services.AddScoped<ICustomerFeedService, CustomerFeedService>();
            services.AddScoped<IMobileAppVersionService, MobileAppVersionService>();

            services.AddScoped<IOfficeServiceTypesApplicationService, OfficeServiceTypesApplicationService>();

            services.AddScoped<IExpenseTypesApplicationService, ExpenseTypesApplicationService>();

            services.AddScoped<IProposalsApplicationService, ProposalsApplicationService>();

            services.AddScoped<IEmailActivityLogApplicationService, EmailActivityLogApplicationService>();

            services.AddScoped<IPDFGeneratorApplicationService, PDFGeneratorApplicationService>();

            services.AddScoped<IInspectionsApplicationService, InspectionsApplicationService>();

            services.AddScoped<IExpensesApplicationService, ExpensesApplicationService>();

            services.AddScoped<IRevenuesApplicationService, RevenuesApplicationService>();

            services.AddScoped<ICalendarApplicationService, CalendarApplicationService>();

            services.AddScoped<IPreCalendarApplicationService, PreCalendarApplicationService>();

            services.AddScoped<ICompanySettingsApplicationService, CompanySettingsApplicationService>();

            services.AddScoped<IScheduleCategoriesApplicationService, ScheduleCategoriesApplicationService>();

            services.AddScoped<ITicketActivityLogApplicationService, TicketActivityLogApplicationService>(); //MG-15

            //  services.AddScoped<IScheduleSubCategoriesApplicationService, ScheduleSubCategoriesApplicationService>();

            services.AddScoped<IFreshdeskApplicationService, FreshdeskApplicationService>();

            services.AddScoped<ITicketCategoriesApplicationService, TicketCategoriesApplicationService>();

            services.AddScoped<ITagsApplicationService, TagsApplicationService>();

            services.AddScoped<IWorkOrderServiceCategoriesApplicationService, WorkOrderServiceCategoriesApplicationService>();

            services.AddScoped<IGMailApiService, GMailApiService>();

            services.AddScoped<IOAuth2Service, OAuth2Service>();

            // Azure service
            services.AddTransient<IAzureStorage, AzureStorage>();

            return services;
        }
    }
}
