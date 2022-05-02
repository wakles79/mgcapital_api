using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskSummaryViewModel : EntityViewModel, IEntityParentViewModel<WorkOrderTaskBaseViewModel>
    {
        public string OriginWorkOrderId { get; set; }

        public string ClonePath { get; set; }

        public int Number { get; set; }

        public IList<WorkOrderTaskBaseViewModel> Children1 { get; set; }

        public IEnumerable<WorkOrderTaskBaseViewModel> Tasks => Children1 as IList<WorkOrderTaskBaseViewModel>;

        public bool ClientApproved { get; set; }

        public bool ScheduleDateConfirmed { get; set; }

        public int NotesCount { get; set; }

        public int TasksCount { get; set; }

        public int TasksDoneCount { get; set; }

        public int StatusId { get; set; }

        public int IsExpired { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public int EpochScheduleDate => this.ScheduleDate.HasValue ? this.ScheduleDate.Value.ToEpoch() : 0;

        public DateTime? DueDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? this.DueDate.Value.ToEpoch() : 0;

        public double TotalBill { get; set; }

        public BillingDateType BillingDateType { get; set; }

        public string BillingDateTypeName => this.BillingDateType.ToString().SplitCamelCase();

        public bool SendRequesterNotifications { get; set; }

        public bool SendPropertyManagersNotifications { get; set; }

        public bool Unscheduled { get; set; }

        public int SequenceId { get; set; }

        /// <summary>
        /// value indicating the position in the sequence in descending order
        /// </summary>
        public int SequencePosition { get; set; }

        public int ElementsInSequence { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
