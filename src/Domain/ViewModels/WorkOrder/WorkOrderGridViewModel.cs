using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderGridViewModel
    {
        public int ID { get; set; }

        public Guid Guid { get; set; }

        public DateTime DateSubmitted { get; set; }

        public int EpochDateSubmitted => this.DateSubmitted.ToEpoch();

        public DateTime? DueDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? this.DueDate.Value.ToEpoch() : 0;

        public string Location { get; set; }

        public int Number { get; set; }

        public int StatusId { get; set; }

        public int NotesCount { get; set; }

        public int TasksCount { get; set; }

        public int TasksDoneCount { get; set; }

        public int TasksBillableCount { get; set; }

        /// <summary>
        /// value indicating the position in the sequence in descending order
        /// </summary>
        public int SequencePosition { get; set; }

        public int ElementsInSequence { get; set; }

        public string Description { get; set; }

        public string RequesterFullName { get; set; }

        public string RequesterEmail { get; set; }

        public string AdministratorFullName { get; set; }

        public string AssignedEmployeeFullName { get; set; }

        public string BuildingName { get; set; }

        public int IsExpired { get; set; }

        public int AttachmentsCount { get; set; }

        public string OriginWorkOrderId { get; set; }

        public string ClonePath { get; set; }

        /// <summary>
        ///     Indicates if the WO can be closed,
        ///     in other words if Day(NOW) >= Day(DueDate)
        /// </summary>
        public bool IsCloseable => this.DueDate.HasValue ? DateTime.UtcNow.Date >= this.DueDate.Value.Date : false;

        public string ClosingNotes { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating if the closing notes will have a different treatment
        /// </summary>
        public bool FollowUpOnClosingNotes { get; set; }

        public bool IsActive { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }

        public bool SendRequesterNotifications { get; set; }

        public bool SendPropertyManagersNotifications { get; set; }

        public int? CalendarItemFrequencyId { get; set; }

        public int SequenceId { get; set; }

        public int? TicketId { get; set; }
    }
}
