using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionReportSendEmailViewModel: EntityViewModel
    {
        /// <summary>
        /// Full name and email of additional recipients
        /// </summary>
        public IEnumerable<InspectionAdditionalRecipientViewModel> AdditionalRecipients { get; set; }
    }
}
