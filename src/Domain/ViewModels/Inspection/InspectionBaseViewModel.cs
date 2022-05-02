using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Utils;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionBaseViewModel : EntityViewModel
    {
        public int Number { get; set; }

        public DateTime? SnoozeDate { get; set; }

        public int EpochSnoozeDate => this.SnoozeDate.HasValue ? this.SnoozeDate.Value.ToEpoch() : 0;

        public int BuildingId { get; set; }

        public int EmployeeId { get; set; }

        public int Stars { get; set; }

        public DateTime? DueDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue? this.DueDate.Value.ToEpoch() : 0 ;

        public DateTime? CloseDate { get; set; }

        public string BeginNotes { get; set; }

        public string ClosingNotes { get; set; }

        public int Score { get; set; }

        public InspectionStatus Status { get; set; }

        public bool AllowPublicView { get; set; }
    }
}
