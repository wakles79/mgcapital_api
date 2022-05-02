using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class ModulePermissionsSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Permissions.Count() > 0)
            {
                return;
            }

            var newPermissions = new List<Permission>()
            {
                // Dashboard
                new Permission()
                {
                    Name ="ViewMobileAppVersions",
                    Module = ApplicationModule.Dashboard,
                    Type = ActionType.Read
                },

                // Inbox
                new Permission()
                {
                    Name ="ReadTickets",
                    Module = ApplicationModule.Inbox,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name ="AddTickets",
                    Module = ApplicationModule.Inbox,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="UpdateTickets",
                    Module = ApplicationModule.Inbox,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="ConvertTickets",
                    Module = ApplicationModule.Inbox,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="DeleteTickets",
                    Module = ApplicationModule.Inbox,
                    Type = ActionType.Write
                },

                // WorkOrder
                new Permission()
                {
                    Name ="ReadWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name ="AddWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="UpdateWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="UpdateSnoozeDateWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="CloneWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="CloseWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="DeleteWorkOrders",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                //new Permission()
                //{
                //    Name ="ReadWorkOrdersNotes",
                //    Module = ApplicationModule.WorkOrder,
                //    Type = ActionType.Read
                //},
                new Permission()
                {
                    Name ="AddWorkOrdersNotes",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="UpdateWorkOrdersNotes",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="DeleteWorkOrdersNotes",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                //new Permission()
                //{
                //    Name ="ReadWorkOrdersTask",
                //    Module = ApplicationModule.WorkOrder,
                //    Type = ActionType.Read
                //},
                new Permission()
                {
                    Name ="AddWorkOrdersTasks",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="UpdateWorkOrdersTasks",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="DeleteWorkOrdersTasks",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="DeleteWorkOrdersAttachment",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name ="ReadWorkOrderTaskBillingInformationFromDetail",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Read
                },

                // DailyReport
                new Permission()
                {
                    Name = "ReadDailyReports",
                    Module = ApplicationModule.DailyReport,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "UpdateDailyReports",
                    Module = ApplicationModule.DailyReport,
                    Type = ActionType.Write
                },

                // BillableReport
                new Permission()
                {
                    Name = "ReadWorkOrderBillableReport",
                    Module = ApplicationModule.BillableReport,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddWorkOrderBillableReport",
                    Module = ApplicationModule.BillableReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateWorkOrderBillableReport",
                    Module = ApplicationModule.BillableReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteWorkOrderBillableReport",
                    Module = ApplicationModule.BillableReport,
                    Type = ActionType.Write
                },

                // CleaningReport
                new Permission()
                {
                    Name = "ReadCleaningReports",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddCleaningReports",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateCleaningReports",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteCleaningReports",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "SendCleaningReportsEmails",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "ReadCleaningReportItems",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddCleaningReportItems",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateCleaningReportItems",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteCleaningReportItems",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteCleaningReportAttachment",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                //new Permission()
                //{
                //    Name = "ReadCleaningReportNotes",
                //    Module = ApplicationModule.CleaningReport,
                //    Type = ActionType.Read
                //},
                new Permission()
                {
                    Name = "AddCleaningReportNotes",
                    Module = ApplicationModule.CleaningReport,
                    Type = ActionType.Write
                },
                //new Permission()
                //{
                //    Name = "UpdateCleaningReportNotes",
                //    Module = ApplicationModule.CleaningReport,
                //    Type = ActionType.Write
                //},
                //new Permission()
                //{
                //    Name = "DeleteCleaningReportNotes",
                //    Module = ApplicationModule.CleaningReport,
                //    Type = ActionType.Write
                //},

                // Inspections
                new Permission()
                {
                    Name = "ReadInspections",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddInspections",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateInspections",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteInspections",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "SendInspectionByEmail",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddInspectionItem",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateInspectionItem",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteInspectionItem",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "CloseInspectionItemDirectly",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddTicketFromInspectionItem",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteInspectionItemAttachment",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddInspectionNote",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteInspectionNote",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateInspectionNote",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "CloseInspectionItemTask",
                    Module = ApplicationModule.Inspections,
                    Type = ActionType.Write
                },

                // Calendar
                new Permission()
                {
                    Name = "ReadCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "ReadInspectionFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddInspectionFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateInspectionFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "ReadTicketFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddTicketFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateTicketFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "ReadWorkOrderFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddWorkOrderFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateWorkOrderFromCalendar",
                    Module = ApplicationModule.Calendar,
                    Type = ActionType.Write
                },

                // Scheduled
                new Permission()
                {
                    Name = "ReadPreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddPreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdatePreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeletePreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddTicketFromPreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddWorkOrderFromPreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddInspectionFromPreCalendar",
                    Module = ApplicationModule.Scheduled,
                    Type = ActionType.Write
                },

                // Buildings
                new Permission()
                {
                    Name = "ReadBuildings",
                    Module = ApplicationModule.Buildings,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddBuildings",
                    Module = ApplicationModule.Buildings,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateBuildings",
                    Module = ApplicationModule.Buildings,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateBuildingAddress",
                    Module = ApplicationModule.Buildings,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteBuildings",
                    Module = ApplicationModule.Buildings,
                    Type = ActionType.Write
                },

                // Proposals
                new Permission()
                {
                    Name = "ReadProposals",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddProposals",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateProposals",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteProposals",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "SendProposalByEmail",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddProposalService",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateProposalService",
                    Module = ApplicationModule.Proposals,
                    Type = ActionType.Write
                },

                // Budgets
                new Permission()
                {
                    Name = "ReadBudgets",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddBudgets",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateBudgets",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteBudgets",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddBudgetRevenue",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateBudgetRevenue",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteBudgetRevenue",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddBudgetExpense",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateBudgetExpense",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteBudgetExpense",
                    Module = ApplicationModule.Budgets,
                    Type = ActionType.Write
                },

                // Revenues
                new Permission()
                {
                    Name = "ReadRevenues",
                    Module = ApplicationModule.Revenues,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddRevenues",
                    Module = ApplicationModule.Revenues,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateRevenues",
                    Module = ApplicationModule.Revenues,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteRevenues",
                    Module = ApplicationModule.Revenues,
                    Type = ActionType.Write
                },

                // Expenses
                new Permission()
                {
                    Name = "ReadExpenses",
                    Module = ApplicationModule.Expenses,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddExpenses",
                    Module = ApplicationModule.Expenses,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateExpenses",
                    Module = ApplicationModule.Expenses,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteExpenses",
                    Module = ApplicationModule.Expenses,
                    Type = ActionType.Write
                },

                // CompanySettings
                new Permission()
                {
                    Name = "ReadCompanySettings",
                    Module = ApplicationModule.CompanySettings,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "UpdateCompanySettings",
                    Module = ApplicationModule.CompanySettings,
                    Type = ActionType.Write
                },

                // Roles
                new Permission()
                {
                    Name = "ReadPermissionsRoles",
                    Module = ApplicationModule.Roles,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "UpdatePermissionsRoles",
                    Module = ApplicationModule.Roles,
                    Type = ActionType.Write
                },

                // Services
                new Permission()
                {
                    Name = "ReadServices",
                    Module = ApplicationModule.Services,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddServices",
                    Module = ApplicationModule.Services,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateServices",
                    Module = ApplicationModule.Services,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteServices",
                    Module = ApplicationModule.Services,
                    Type = ActionType.Write
                },

                // ContractServicesCatalog
                new Permission()
                {
                    Name = "ReadContractServicesCatalog",
                    Module = ApplicationModule.ContractServicesCatalog,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddContractServicesCatalog",
                    Module = ApplicationModule.ContractServicesCatalog,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateContractServicesCatalog",
                    Module = ApplicationModule.ContractServicesCatalog,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteContractServicesCatalog",
                    Module = ApplicationModule.ContractServicesCatalog,
                    Type = ActionType.Write
                },

                // ExpensesTypesCatalog
                new Permission()
                {
                    Name = "ReadExpensesTypesCatalog",
                    Module = ApplicationModule.ExpensesTypesCatalog,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddExpensesTypesCatalog",
                    Module = ApplicationModule.ExpensesTypesCatalog,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateExpensesTypesCatalog",
                    Module = ApplicationModule.ExpensesTypesCatalog,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteExpensesTypesCatalog",
                    Module = ApplicationModule.ExpensesTypesCatalog,
                    Type = ActionType.Write
                },

                // Users
                new Permission()
                {
                    Name = "ReadUsers",
                    Module = ApplicationModule.Users,
                    Type = ActionType.Read
                },
                //new Permission()
                //{
                //    Name = "AddUsers",
                //    Module = ApplicationModule.Users,
                //    Type = ActionType.Write
                //},
                new Permission()
                {
                    Name = "UpdateUsers",
                    Module = ApplicationModule.Users,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteUsers",
                    Module = ApplicationModule.Users,
                    Type = ActionType.Write
                },

                // Contacts
                new Permission()
                {
                    Name = "ReadContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UnassignContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AssignContacts",
                    Module = ApplicationModule.Contacts,
                    Type = ActionType.Write
                },

                // ManagementCo
                new Permission()
                {
                    Name = "ReadManagementCo",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name = "AddManagementCo",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateManagementCo",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteManagementCo",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddManagementCoPhone",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateManagementCoPhone",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteManagementCoPhone",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "AddManagementCoAddress",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "UpdateManagementCoAddress",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name = "DeleteManagementCoAddress",
                    Module = ApplicationModule.ManagementCo,
                    Type = ActionType.Write
                },

                // EmailAcitivity
                new Permission()
                {
                    Name = "ReadEmailActivity",
                    Module = ApplicationModule.EmailActivity,
                    Type = ActionType.Read
                },

                // Scheduled Work Order Categories
                new Permission()
                {
                    Name="ReadScheduledWorkOrderCategories",
                    Module = ApplicationModule.ScheduledWorkOrderCategories,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name="AddScheduledWorkOrderCategories",
                    Module = ApplicationModule.ScheduledWorkOrderCategories,
                    Type = ActionType.Write
                },
                new Permission()
                {
                    Name="UpdateScheduledWorkOrderCategories",
                    Module = ApplicationModule.ScheduledWorkOrderCategories,
                    Type = ActionType.Write
                },

                // BudgetSettings
                new Permission()
                {
                    Name="ReadBudgetSettings",
                    Module = ApplicationModule.BudgetSettings,
                    Type = ActionType.Read
                },
                new Permission()
                {
                    Name="UpdateBudgetSettings",
                    Module = ApplicationModule.BudgetSettings,
                    Type = ActionType.Write
                },

                // Work Order Service Catalog
                //new Permission()
                //{
                //    Name= "ReadWorkOrderCategories",
                //    Module = ApplicationModule.WorkOrderServicesCatalog,
                //    Type = ActionType.Read
                //},
                new Permission()
                {
                    Name= "ReadWorkOrderServices",
                    Module = ApplicationModule.WorkOrderServicesCatalog,
                    Type = ActionType.Read
                },
                //new Permission(){
                //    Name = "AddWorkOrderCategories",
                //    Module = ApplicationModule.WorkOrderServicesCatalog,
                //    Type = ActionType.Write
                //},
                //new Permission(){
                //    Name = "UpdateWorkOrderCategories",
                //    Module = ApplicationModule.WorkOrderServicesCatalog,
                //    Type = ActionType.Write
                //},
                new Permission(){
                    Name = "AddWorkOrderServices",
                    Module = ApplicationModule.WorkOrderServicesCatalog,
                    Type = ActionType.Write
                },
                new Permission(){
                    Name = "UpdateWorkOrderServices",
                    Module = ApplicationModule.WorkOrderServicesCatalog,
                    Type = ActionType.Write
                },
                new Permission(){
                    Name = "WorkOrderTaskBillingNotes",
                    Module = ApplicationModule.WorkOrder,
                    Type = ActionType.Write
                },
            };

            context.Permissions.AddRange(newPermissions);
        }
    }
}
