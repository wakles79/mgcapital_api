using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Freshdesk;
using System.Collections.Generic;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class TicketFreshdeskSummaryViewModel : EntityViewModel
    {
        public FreshdeskTicketBaseViewModel Ticket { get; set; }

        public IEnumerable<ConversationBaseViewModel> Conversations { get; set; }

        public TicketFreshdeskSummaryViewModel()
        {
            this.Conversations = new HashSet<ConversationBaseViewModel>();
        }
    }
}
