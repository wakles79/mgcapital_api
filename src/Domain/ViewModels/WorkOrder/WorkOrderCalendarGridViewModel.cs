using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderCalendarGridViewModel : EntityViewModel
    {
        public string OriginWorkOrderId { get; set; }

        public string ClonePath { get; set; }

        public int Number { get; set; }

        public string Description { get; set; }

        public string BuildingName { get; set; }

        public string Location { get; set; }

        public string RequesterFullName { get; set; }

        public DateTime DateSubmitted { get; set; }

        public int EpochDateSubmitted => this.DateSubmitted.ToEpoch();

        public DateTime? DueDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? this.DueDate.Value.ToEpoch() : 0;

        public int IsExpired { get; set; }

        public bool SendRequesterNotifications { get; set; }

        public bool SendPropertyManagersNotifications { get; set; }

        public int NotesCount { get; set; }

        public int TasksCount { get; set; }

        public int TasksDoneCount { get; set; }

        public int TasksBillableCount { get; set; }

        public int AttachmentsCount { get; set; }

        public int StatusId { get; set; }

        /// <summary>
        /// value indicating the position in the sequence in descending order
        /// </summary>
        public int SequencePosition { get; set; }

        public int ElementsInSequence { get; set; }

        public WorkOrderType Type { get; set; }

        public Guid Guid { get; set; }

        public int? CalendarItemFrequencyId { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int EpochScheduleDate => this.ScheduleDate.HasValue ? this.ScheduleDate.Value.ToEpoch() : 0;

        [IgnoreDataMember]
        public int Count { get; set; }

        public int ScheduleCategoryId { get; set; }

        public int ScheduleSubCategoryId {get; set;}

        public string ScheduleCategoryName { get; set; }

        public string ScheduleSubCategoryName { get; set; }

        public bool ClientApproved { get; set; }

        public bool ScheduleDateConfirmed { get; set; }

        public BillingDateType BillingDateType { get; set; }

        public string BillingDateTypeName => this.BillingDateType.ToString().SplitCamelCase();

        public double TotalBill { get; set; }

        public ScheduleCategoryColor? Color { get; set; }

        public string ColorName => this.Color.HasValue ? this.Color.ToString() : "black";

        public bool Unscheduled { get; set; }

        public int? WorkOrderScheduleSettingId { get; set; }
    }
}
