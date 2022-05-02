using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportSendEmailViewModel : EntityViewModel
    {
        /// <summary>
        /// Full name and email of additional recipients
        /// </summary>
        public IEnumerable<CleaningReportAdditionalRecipientViewModel> AdditionalRecipients { get; set; }
    }
}
