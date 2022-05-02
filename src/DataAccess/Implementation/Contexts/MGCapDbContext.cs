// -----------------------------------------------------------------------
// <copyright file="MGCapDbContext.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Context;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Context
{
    /// <summary>
    ///     Implementation of the <see cref="DbContext"/>.
    ///     Declare the different <see cref="DbSet{TEntity}"/> that
    ///     will be use for the creation of the tables and manipulation
    ///     of its data.
    /// <remarks>
    ///     If necessary, override some of the <see cref="DbContext"/>
    ///     methods for adding functionalities before the base implementation
    ///     is called.
    /// </remarks>
    /// </summary>
    public class MGCapDbContext : DbContext, IMGCapDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MGCapDbContext"/> class.
        /// </summary>
        /// <param name="opts">The options to be used by the <see cref="DbContext"/></param>
        public MGCapDbContext(DbContextOptions<MGCapDbContext> opts) : base(opts)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                        .Property(w => w.IsExpired)
                        .HasComputedColumnSql("CASE WHEN [DueDate] IS NULL THEN 0 WHEN (CASE WHEN [DueDate] = '0001-01-01 00:00:00.0000000' THEN '3000-01-01' ELSE CAST([DueDate] AS DATETIME) END) < GETUTCDATE() THEN 1 ELSE 0 END");
            modelBuilder.Entity<WorkOrderEmployee>()
                        .HasKey(k => new { k.EmployeeId, k.WorkOrderId, k.Type });
            modelBuilder.Entity<PushNotificationConvert>()
                            .HasKey(k => new { k.PushNotificationId, k.EmployeeId });
            modelBuilder.Entity<ContactAddress>()
                        .HasKey(k => new { k.AddressId, k.ContactId });
            modelBuilder.Entity<VendorAddress>()
                        .HasKey(k => new { k.AddressId, k.VendorId });
            modelBuilder.Entity<CustomerAddress>()
                        .HasKey(k => new { k.AddressId, k.CustomerId });
            modelBuilder.Entity<CustomerContact>()
                        .HasKey(k => new { k.ContactId, k.CustomerId });
            modelBuilder.Entity<BuildingContact>()
                        .HasKey(k => new { k.ContactId, k.BuildingId });
            modelBuilder.Entity<BuildingEmployee>()
                        .HasKey(k => new { k.EmployeeId, k.BuildingId, k.Type });
            modelBuilder.Entity<CustomerEmployee>()
                        .HasKey(k => new { k.EmployeeId, k.CustomerId });
            modelBuilder.Entity<CustomerCustomerGroup>()
                        .HasKey(k => new { k.CustomerId, k.CustomerGroupId });
            modelBuilder.Entity<VendorContact>()
                        .HasKey(k => new { k.ContactId, k.VendorId });
            modelBuilder.Entity<VendorVendorGroup>()
                        .HasKey(k => new { k.VendorId, k.VendorGroupId });
            modelBuilder.Entity<EmployeePermission>()
                        .HasKey(k => new { k.EmployeeId, k.PermissionId });
            modelBuilder.Entity<PermissionRole>()
                        .HasKey(k => new { k.PermissionId, k.RoleId });
            modelBuilder.Entity<Address>()
                       .Property(a => a.FullAddress)
                       .HasComputedColumnSql("CONCAT(AddressLine1 + ' ', AddressLine2 + ' ', City + ' ', State + ' ', ZipCode + ' ', CountryCode)");
            modelBuilder.Entity<WorkOrderActivityLog>()
                        .Property(w => w.ChangeLog)
                        .HasConversion(
                                log => JsonConvert.SerializeObject(log),
                                log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<CleaningReportActivityLog>()
                        .Property(c => c.ChangeLog)
                        .HasConversion(
                                log => JsonConvert.SerializeObject(log),
                                log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<CleaningReportActivityLog>()
                        .Property(c => c.ItemLog)
                        .HasConversion(
                                log => JsonConvert.SerializeObject(log),
                                log => JsonConvert.DeserializeObject<IList<ItemLogEntry>>(log));
            modelBuilder.Entity<CleaningReportActivityLog>()
                        .Property(c => c.EmailLog)
                        .HasConversion(
                                log => JsonConvert.SerializeObject(log),
                                log => JsonConvert.DeserializeObject<IList<EmailLogEntry>>(log));
            modelBuilder.Entity<EmailActivityLog>()
                        .Property(c => c.Cc)
                        .HasConversion(
                            cc => JsonConvert.SerializeObject(cc),
                            cc => JsonConvert.DeserializeObject<IList<EmailLogEntry>>(cc));
            modelBuilder.Entity<InspectionActivityLog>()
                        .Property(i => i.ChangeLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<InspectionActivityLog>()
                        .Property(i => i.ItemLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ItemLogEntry>>(log));
            modelBuilder.Entity<InspectionActivityLog>()
                        .Property(i => i.EmailLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<EmailLogEntry>>(log));
            modelBuilder.Entity<ContractActivityLog>()
                        .Property(c => c.ChangeLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<ContractActivityLog>()
                        .Property(c => c.ItemLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ItemLogEntry>>(log));
            modelBuilder.Entity<BuildingActivityLog>()
                        .Property(b => b.ChangeLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<Ticket>()
                .Property(w => w.Data)
                .HasConversion(
                    d => JsonConvert.SerializeObject(d),
                    d => JsonConvert.DeserializeObject<Dictionary<string, string>>(d));
            modelBuilder.Entity<CalendarItemFrequency>()
                .Property(c => c.Months)
                .HasConversion(
                    c => JsonConvert.SerializeObject(c),
                    c => JsonConvert.DeserializeObject<IEnumerable<int>>(c));
            modelBuilder.Entity<WorkOrderScheduleSetting>()
                .Property(w => w.Days)
                .HasConversion(
                    d => JsonConvert.SerializeObject(d),
                    d => JsonConvert.DeserializeObject<IEnumerable<int>>(d));
            modelBuilder.Entity<TicketActivityLog>()
                        .Property(i => i.ChangeLog)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<ChangeLogEntry>>(log));
            modelBuilder.Entity<TicketActivityLog>()
                        .Property(i => i.ChangeSummary)
                        .HasConversion(
                            log => JsonConvert.SerializeObject(log),
                            log => JsonConvert.DeserializeObject<IList<TicketChangeSummaryEntry>>(log));

            // prevent to repeat freshdesk api key and agent id
            modelBuilder.Entity<CompanySettings>()
                .HasIndex(e => e.FreshdeskDefaultApiKey)
                .IsUnique(true);

            // TODO: apply global filter to all ISoftDeletable entities
            modelBuilder.Entity<Ticket>().HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => new { c.Code ,c.CompanyId })
                .IsUnique();
                
            #region Permissions
            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.Name, p.Module, p.Type })
                .IsUnique();
            #endregion
            
            #region Employees
            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.Email, e.CompanyId })
                .IsUnique();
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Companies</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<Company> Companies { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Employees</value> table
        ///     and handling its data.
        /// </summary>  
        public virtual DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Contacts</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<Contact> Contacts { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>ContactPhones</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<ContactPhone> ContactPhones { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>ContactEmails</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<ContactEmail> ContactEmails { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Addresses</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>ContactAddresses</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<ContactAddress> ContactAddresses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Departments</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Department> Departments { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Vendors</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Vendor> Vendors { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorPhone</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<VendorPhone> VendorPhones { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorEmail</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<VendorEmail> VendorEmails { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorAddress</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<VendorAddress> VendorAddresses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Customer</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerPhone</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerPhone> CustomerPhones { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerAddress</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerGroup</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerCertificate</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerCertificate> CustomerCertificates { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorContact</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<VendorContact> VendorContacts { get; set; }
        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerContact</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerContact> CustomerContacts { get; set; }
        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerEmployee</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerEmployee> CustomerEmployees { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CustomerCustomerGroup</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CustomerCustomerGroup> CustomerCustomerGroups { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Franchises</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<Franchise> Franchises { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorGroups</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<VendorGroup> VendorGroups { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>VendorVendorGroups</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<VendorVendorGroup> VendorVendorGroups { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrders</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderStatusLog</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderStatusLogEntry> WorkOrderStatusLog { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderTasks</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderTask> WorkOrderTasks { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderNotes</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderNote> WorkOrderNotes { get; set; }

        public virtual DbSet<WorkOrderAttachment> WorkOrderAttachments { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderSource</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderSource> WorkOrderSources { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderEmployee</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderEmployee> WorkOrderEmployees { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Buildings</value> table
        ///     and handling its data.
        /// </summary>
        public virtual DbSet<Building> Buildings { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>BuildingContacts</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<BuildingContact> BuildingContacts { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>BuildingEmployees</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<BuildingEmployee> BuildingEmployees { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Services</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Service> Services { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Roles</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Permissions</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>EmployeePremissions</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<EmployeePermission> EmployeePermissions { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>PermissionRoles</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<PermissionRole> PermissionRoles { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>Contract</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<Contract> Contracts { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>WorkOrderNotificationTemplates</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<WorkOrderNotificationTemplate> WorkOrderNotificationTemplates { get; set; }

        public virtual DbSet<PushNotification> PushNotifications { get; set; }

        public virtual DbSet<PushNotificationFilter> PushNotificationFilters { get; set; }

        public virtual DbSet<PushNotificationConvert> PushNotificationConverts { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CleaningReports</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CleaningReport> CleaningReports { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CleaningReportItems</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CleaningReportItem> CleaningReportItems { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEnity}"/> for the creation of the <value>CleaningReportItemAttachments</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<CleaningReportItemAttachment> CleaningReportItemAttachments { get; set; }

        public virtual DbSet<CustomerUser> CustomerUsers { get; set; }

        public virtual DbSet<Ticket> Tickets { get; set; }

        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }

        public virtual DbSet<MobileAppVersion> MobileAppVersions { get; set; }

        public virtual DbSet<CleaningReportNote> CleaningReportNotes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{OfficeServiceType}"/> for the creation of the <value>OfficeServiceTypes</value> table
        ///  and handling its data.
        /// </summary>
        public virtual DbSet<OfficeServiceType> OfficeServiceTypes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ContractItem}"/> for the creation of the <value>ContractItem</value> table and handling its data
        /// </summary>
        public virtual DbSet<ContractItem> ContractItems { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ExpenseSubcategory}"/> for the creation of the <value>ExpenseSubcategory</value> table and handling its data
        /// </summary>
        public virtual DbSet<ExpenseSubcategory> ExpenseSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ExpenseType}"/> for the creation of the <value>ExpenseType</value> table and handling its data
        /// </summary>
        public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ContractExpense}"/> for the creation of the <value>ContractExpenses</value> table and handling its data
        /// </summary>
        public virtual DbSet<ContractExpense> ContractExpenses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{Proposal}"/> for the creation of the <value>Proposals</value> table and handling its data
        /// </summary>
        public virtual DbSet<Proposal> Proposals { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ProposalService}"/> for the creation of the <value>ProposalServices</value> table and handling its data
        /// </summary>
        public virtual DbSet<ProposalService> ProposalServices { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{Inspection}"/> for the creation of the <value>Inspections</value> table and handling its data
        /// </summary>
        public virtual DbSet<Inspection> Inspections { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionItem}"/> for the creation of the <value>InspectionItems</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionItem> InspectionItems { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionItemAttachment}"/> for the creation of the <value>InspectionItemAttachments</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionItemAttachment> InspectionItemAttachments { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{Expense}"/> for the creation of the <value>Expenses</value> table and handling its data
        /// </summary>
        public virtual DbSet<Expense> Expenses { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionItemTicket}"/> for the creation of the <value>InspectionItemTickets</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionItemTicket> InspectionItemTickets { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionItemTask}"/> for the creation of the <value>InspectionItemTasks</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionItemTask> InspectionItemTasks { get; set; }

        /// <summary> 
        /// Gets or sets a <see cref="DbSet{ContractOfficeSpace}"/> for the creation of the <value>ContractOfficeSpaces</value> table and handling its data 
        /// </summary> 
        public virtual DbSet<ContractOfficeSpace> ContractOfficeSpaces { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{Revenue}"/> for the creation of the <value>Revenues</value> table and handling its data
        /// </summary>
        public virtual DbSet<Revenue> Revenues { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{PreCalendar}"/> for the creation of the <value>PreCalendar</value> table and handling its data
        /// </summary>
        public virtual DbSet<PreCalendar> PreCalendar { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionNote}"/> for the creation of the <value>InspectionNotes</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionNote> InspectionNotes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{InspectionItemNote}"/> for the creation of the <value>InspectionItemNotes</value> table and handling its data
        /// </summary>
        public virtual DbSet<InspectionItemNote> InspectionItemNotes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{CompanySettings}"/> for the creation of the <value>CompanySettings</value> table and handling its data
        /// </summary>
        public virtual DbSet<CompanySettings> CompanySettings { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{PreCalendarTask}"/> for the creation of the <value>PreCalendarTasks</value> table and handling its data
        /// </summary>
        public virtual DbSet<PreCalendarTask> PreCalendarTasks { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ScheduleSettingCategory}"/> for the creation of the <value>ScheduleSettingCategories</value> table and handling its data
        /// </summary>
        public virtual DbSet<ScheduleSettingCategory> ScheduleSettingCategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ScheduleSettingSubCategory}"/> for the creation of the <value>ScheduleSettingSubCategories</value> table and handling its data
        /// </summary>
        public virtual DbSet<ScheduleSettingSubCategory> ScheduleSettingSubCategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ContractNote}"/> for the creation of the <value>ContractNotes</value> table and handling its data
        /// </summary>
        public virtual DbSet<ContractNote> ContractNotes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ContractActivityLogNote}"/> for the creation of the <value>ContractActivityLogNotes</value> table and handling its data
        /// </summary>
        public virtual DbSet<ContractActivityLogNote> ContractActivityLogNotes { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TicketCategory}"/> for the creation of the <value>TicketCategories</value> table and handling its data
        /// </summary>
        public virtual DbSet<TicketCategory> TicketCategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{Tag}"/> for the creation of the <value>Tags</value> table and handling its data
        /// </summary>
        public virtual DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TicketTag}"/> for the creation of the <value>TicketTags</value> table and handling its data
        /// </summary>
        public virtual DbSet<TicketTag> TicketTags { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{WorkOrderTaskAttachment}"/> for the creation of the <value>WorkOrderTaskAttachments</value> table and handling its data
        /// </summary>
        public virtual DbSet<WorkOrderTaskAttachment> WorkOrderTaskAttachments { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{WorkOrderServiceCategory}"/> for the creation of the <value>WorkOrderServiceCategories</value> table and handling its data
        /// </summary>
        public virtual DbSet<WorkOrderServiceCategory> WorkOrderServiceCategories { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{WorkOrderService}"/> for the creation of the <value>WorkOrderServices</value> table and handling its data
        /// </summary>
        public virtual DbSet<WorkOrderService> WorkOrderServices { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{ConvertedTicket}"/> for the creation of the <value>ConvertedTickets</value> table and handling its data
        /// </summary>
        public virtual DbSet<ConvertedTicket> ConvertedTickets { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TicketEmailHistory}"/> for the creation of the <value>TicketEmailHistory</value> table and handling its data
        /// </summary>
        public virtual DbSet<TicketEmailHistory> TicketEmailHistory { get; set; }
    }
}
