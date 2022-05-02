using MGCap.Domain.Utils;
using System;

namespace MGCap.Domain.ViewModels.Common
{
    public class AuditableEntityViewModel : EntityViewModel
    {
        public DateTime CreatedDate { get; set; }
        /// <summary>
        ///     Gets updated date in Unix time
        /// </summary>
        public int EpochCreatedDate => this.CreatedDate.ToEpoch();
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        ///     Gets updated date in Unix time
        /// </summary>
        public int EpochUpdatedDate => this.UpdatedDate.ToEpoch();
    }
}
