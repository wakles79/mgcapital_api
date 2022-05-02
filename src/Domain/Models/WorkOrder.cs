// <copyright file="WorkOrder.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class WorkOrder : AuditableCompanyEntity, IDocumentEntity<int>
    {
        public int? BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [MaxLength(128)]
        public string Location { get; set; }

        /// <summary>
        ///     Gets or sets the current WO Number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Gets or sets the WO Description, mostly email bodies from
        ///     requesters
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Notes when a WO is being closed
        /// </summary>
        public string ClosingNotes { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating if the closing notes will have a different treatment
        /// </summary>
        public bool? FollowUpOnClosingNotes { get; set; }

        /// <summary>
        ///     External closing notes
        /// </summary>
        public WorkOrderClosingNotes AdditionalNotes { get; set; }

        /// <summary>
        ///    Closing Aditional Notes Other  when a WO is being closed
        /// </summary>
        [MaxLength(255)]
        public string ClosingNotesOther { get; set; }

        /// <summary>
        ///     Gets or sets the employee that 'manages' the WO
        /// </summary>
        public int? AdministratorId { get; set; }

        [ForeignKey("AdministratorId")]
        public Employee Administrator { get; set; }

        /// <summary>
        ///     Gets or sets the WO's status
        ///     i.e 'Draft', 'Pending', 'In Progress', 'Complete'
        /// </summary>
        public WorkOrderStatus StatusId { get; set; }

        /// <summary>
        ///     Gets or sets the customer contact
        /// </summary>
        public int? CustomerContactId { get; set; }

        [ForeignKey("CustomerContactId")]
        public Contact CustomerContact { get; set; }

        /// <summary>
        ///     Gets or sets the work order source
        /// </summary>        
        public int? WorkOrderSourceId { get; set; }

        [ForeignKey("WorkOrderSourceId")]
        public WorkOrderSource WorkOrderSource { get; set; }

        public int? OriginWorkOrderId { get; set; }

        /// <summary>
        ///     Gets or sets the work order clone number (0 if it isn't a clone)
        /// </summary>
        public int CloneNumber { get; set; }

        /// <summary>
        ///     Gets or sets the WO's priority
        ///     i.e 'High', 'Medium', 'Low'
        /// </summary>
        public WorkOrderPriority Priority { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating if the system should
        ///     send notifications to the requester
        /// </summary>
        public bool SendRequesterNotifications { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating if the system should
        ///     send notifications to all Property Managers (by default this flag
        ///     should be true)
        /// </summary>
        public bool SendPropertyManagersNotifications { get; set; }

        /// <summary>
        ///     Gets or sets the Due Date of a WO
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        ///     Gets or sets the SnoozeDate of a WO, can be null
        /// </summary>
        public DateTime? SnoozeDate { get; set; }

        /// <summary>
        ///     Could be `Request`, `Complaint`, `Important` and `Other`
        ///     By default should be `Request`
        /// </summary>
        public WorkOrderType Type { get; set; }

        /// <summary>
        ///     Computed SQL column that determines whenever a WO is expired or not
        /// </summary>
        public int IsExpired { get; set; }

        /// <summary>
        ///     Gets or sets id of property managers assigned to the work order for 
        ///     future references
        /// </summary>
        public string PropertyManagersId { get; set; }

        #region Billing Information
        /// <summary>
        ///     Gets or sets work order billing information
        /// </summary>
        public string BillingName { get; set; }

        public string BillingEmail { get; set; }

        public string BillingNote { get; set; }
        #endregion

        #region Requester Fields
        [MaxLength(250)]
        public string FullAddress { get; set; }

        [MaxLength(200)]
        public string RequesterFullName { get; set; }

        [MaxLength(128)]
        public string RequesterEmail { get; set; }

        [MaxLength(13)]
        public string RequesterPhone { get; set; }

        #endregion

        public ICollection<WorkOrderNote> Notes { get; set; }

        public ICollection<WorkOrderTask> Tasks { get; set; }

        public ICollection<WorkOrderAttachment> Attachments { get; set; }

        public WorkOrder()
        {
            this.IsActive = true;
            this.Notes = new HashSet<WorkOrderNote>();
            this.Tasks = new HashSet<WorkOrderTask>();
            this.Attachments = new HashSet<WorkOrderAttachment>();
            this.ClosingNotesOther = string.Empty;
        }

        /// <summary>
        ///     Indicates if the WO can be closed,
        ///     in other words if Day(NOW) >= Day(DueDate)
        /// </summary>

        public bool IsCloseable => this.DueDate.HasValue ? DateTime.UtcNow.Date >= this.DueDate.Value.Date : false;

        public bool IsActive { get; set; }

        public WorkOrderBillingDateType? BillingDateType { get; set; }

        #region Schedule Setting
        public bool ClientApproved { get; set; }

        public bool ScheduleDateConfirmed { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? ScheduleCategoryId { get; set; }

        [ForeignKey("ScheduleCategoryId")]
        public ScheduleSettingCategory ScheduleSettingCategory { get; set; }

        public int? ScheduleSubCategoryId { get; set; }

        [ForeignKey("ScheduleSubCategoryId")]
        public ScheduleSettingSubCategory ScheduleSettingSubCategory { get; set; }

        public int? CalendarItemFrequencyId { get; set; }

        public bool Unscheduled { get; set; }

        public int? WorkOrderScheduleSettingId { get; set; }
        #endregion Schedule Setting
    }
}
