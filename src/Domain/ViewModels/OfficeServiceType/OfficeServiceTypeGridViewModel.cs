using System;
using System.Runtime.Serialization;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.OfficeServiceType
{
    public class OfficeServiceTypeGridViewModel : OfficeServiceTypeBaseViewModel, IGridViewModel
    {
        public Guid Guid { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }

        /// <summary>
        /// Gets the name of the rate type.
        /// </summary>
        /// <value>The name of the rate type.</value>
        public string RateTypeName => this.RateType.ToString().SplitCamelCase();

        public bool IsUsed { get; set; }
    }
}