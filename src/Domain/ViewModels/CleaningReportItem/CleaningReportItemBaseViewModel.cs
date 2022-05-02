using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReportItem
{
    public class CleaningReportItemBaseViewModel : EntityViewModel
    {
        public int CleaningReportId { get; set; }

        public int BuildingId { get; set; }

        [MaxLength(80)]
        public string Location { get; set; }

        [MaxLength(16)]
        public string Time { get; set; }

        public string Observances { get; set; }

        public CleaningReportType Type { get; set; }
    }
}
