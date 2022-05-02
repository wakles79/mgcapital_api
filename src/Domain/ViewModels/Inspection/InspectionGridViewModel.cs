using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionGridViewModel : InspectionBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }

        public int Items { get; set; }

        public int ClosedItems {get;set;}

        public int Tasks { get; set; }

        public int CompletedTasks { get; set; }

        public int NotesCount { get; set; }

        public string BuildingName { get; set; }

        public string EmployeeName { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        public DateTime CreatedDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? DueDate.Value.ToEpoch() : 0;

        public int EpochCloseDate => this.CloseDate.HasValue ? CloseDate.Value.ToEpoch() : 0;
    }
}
