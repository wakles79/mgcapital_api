using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.InspectionItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionReportDetailViewModel : InspectionBaseViewModel
    {
        public string BuildingName { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeEmail { get; set; }

        public string EmployeePhone { get; set; }

        public Guid Guid { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        public IEnumerable<InspectionItemGridViewModel> InspectionItem { get; set; }

        public IEnumerable<InspectionNoteGridViewModel> InspectionNote { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? DueDate.Value.ToEpoch() : 0;

        public int EpochCloseDate => this.CloseDate.HasValue ? CloseDate.Value.ToEpoch() : 0;

        public InspectionReportDetailViewModel()
        {
            this.InspectionItem = new HashSet<InspectionItemGridViewModel>();
            this.InspectionNote = new HashSet<InspectionNoteGridViewModel>();
        }
    }
}
