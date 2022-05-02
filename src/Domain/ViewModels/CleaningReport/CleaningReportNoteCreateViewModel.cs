using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportNoteCreateViewModel : CleanintReportNoteBaseCreateViewModel
    {
        public int CleaningReportId { get; set; }
    }

    public class CleaningReportNoteCreatePublicViewModel : CleanintReportNoteBaseCreateViewModel
    {
        public Guid CleaningReportGuid { get; set; }
    }

    public class CleanintReportNoteBaseCreateViewModel
    {
        [MinLength(1)]
        public string Message { get; set; }

        public CleaningReportNoteDirection Direction { get; set; }
    }
}
