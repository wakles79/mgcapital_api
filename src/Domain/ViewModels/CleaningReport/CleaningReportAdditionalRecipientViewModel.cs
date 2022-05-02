using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportAdditionalRecipientViewModel
    { 
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
