using MGCap.Domain.ViewModels.Common;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Tag
{
    public class TagGridViewModel : AuditableEntityViewModel, IGridViewModel
    {
        public string Description { get; set; }

        /// <summary>
        /// Total of another entities referenced
        /// </summary>
        public int ReferenceCount { get; set; }

        public string HexColor { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
