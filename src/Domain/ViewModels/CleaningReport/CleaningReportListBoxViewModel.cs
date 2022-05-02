using System;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;


namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportListBoxViewModel : ListBoxViewModel
    {

        public DateTime CreatedDate { get; set; }
        
        public int EpochCreatedDate => this.CreatedDate.ToEpoch();

        public string Location { get; set; }
        
        public int CustomerContactId { get; set; }

        public int Number { get; set; }

        private string _to;
        /// <summary>
        ///     Contact Full Name
        /// </summary>
        public string To { get => this._to.RemoveDuplicatedSpaces(); set => this._to = value; }

        private string _from;
        /// <summary>
        ///     Employee Full Name
        /// </summary>
        public string From { get => this._from.RemoveDuplicatedSpaces(); set => this._from = value; }
    }    
}
