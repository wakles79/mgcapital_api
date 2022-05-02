using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportBaseViewModel : AuditableEntityViewModel
    {
        public int? Number { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public int EmployeeId { get; set; }

        [MaxLength(80)]
        public string Location { get; set; }

        public DateTime DateOfService { get; set; }

        public int EpochDateOfService => this.DateOfService.ToEpoch();

        public int Submitted { get; set; }

    }
}
