using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderDailyReportViewModel
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

        public string Description { get; set; }

        public string RequesterFullName { get; set; }

        public string RequesterEmail { get; set; }

        public string OperationsManagerFullName { get; set; }

        public string BuildingName { get; set; }

        public int IsExpired { get; set; }

        public int AttachmentsCount { get; set; }

        public string OriginWorkOrderId { get; set; }

        public string ClonePath { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
