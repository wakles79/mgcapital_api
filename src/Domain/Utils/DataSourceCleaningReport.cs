using MGCap.Domain.ViewModels.CleaningReport;

namespace MGCap.Domain.Utils
{
    public class DataSourceCleaningReport : DataSource<CleaningReportGridViewModel>
    {
        /// <summary>
        /// Gets or sets the number of cleaning reports that are
        /// considered 'replied'
        /// </summary>
        /// <value></value>
        public int RepliedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of cleaning reports that are
        /// considered 'pending to reply'
        /// </summary>
        /// <value></value>
        public int PendingToReplyCount { get; set; }
    }
}
