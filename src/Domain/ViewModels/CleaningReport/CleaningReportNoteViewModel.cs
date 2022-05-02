using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportNoteViewModel : AuditableEntityViewModel
    {
        private string _senderName;

        public int CleaningReportId { get; set; }

        public CleaningReportNoteDirection Direction { get; set; }

        public string SenderName
        {
            get => _senderName.RemoveDuplicatedSpaces();
            set => _senderName = value;
        }

        public string Message { get; set; }
    }
}
