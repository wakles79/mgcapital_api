using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderBaseViewModel : EntityViewModel
    {
        [Required]
        public int BuildingId { get; set; }

        public string Location { get; set; }

        public int? AdministratorId { get; set; }

        //public int? AssignedEmployeeId { get; set; }
        //public int? CustomerContactId { get; set; }
        //public int? RequesterId { get; set; }

        public WorkOrderPriority? Priority { get; set; }

        public bool? SendRequesterNotifications { get; set; }

        public bool? SendPropertyManagersNotifications { get; set; }

        public WorkOrderStatus StatusId { get; set; }

        public int? Number { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? SnoozeDate { get; set; }

        public int EpochSnoozeDate => this.SnoozeDate.HasValue ? this.SnoozeDate.Value.ToEpoch() : 0;

        public int EpochDueDate => this.DueDate.HasValue ? this.DueDate.Value.ToEpoch() : 0;

        public WorkOrderType Type { get; set; }

        public int WorkOrderSourceId { get; set; }

        public string BillingName { get; set; }

        public string BillingEmail { get; set; }

        public string BillingNote { get; set; }

        /// <summary>
        ///     Notes when a WO is being closed
        /// </summary>
        public string ClosingNotes { get; set; }

        /// <summary>
        ///     External closing notes
        /// </summary>
        public WorkOrderClosingNotes AdditionalNotes { get; set; }

        /// <summary>
        ///     Notes when a WO is being closed
        /// </summary>
        public string ClosingNotesOther { get; set; }

        public int? OriginWorkOrderId { get; set; }

        public string ClonePath { get; set; }

        public int? OriginWorkOrderNumber { get; set; }

        public Guid Guid { get; set; }

        /// <summary>
        ///     Indicates if the WO can be closed,
        ///     in other words if Day(NOW) >= Day(DueDate)
        /// </summary>
        public bool IsCloseable => this.DueDate.HasValue ? DateTime.UtcNow >= this.DueDate.Value : false;

        public WorkOrderBillingDateType? BillingDateType { get; set; }

        #region Schedule Setting
        public bool ClientApproved { get; set; }

        public bool ScheduleDateConfirmed { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int? ScheduleCategoryId { get; set; }

        public int? ScheduleSubCategoryId { get; set; }

        public int EpochScheduleDate => this.ScheduleDate.HasValue ? this.ScheduleDate.Value.ToEpoch() : 0;

        public int? CalendarItemFrequencyId { get; set; }

        public bool Unscheduled { get; set; }

        public int? WorkOrderScheduleSettingId { get; set; }
        #endregion Schedule Setting
    }
}
