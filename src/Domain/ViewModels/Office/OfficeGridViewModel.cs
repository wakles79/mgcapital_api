using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Office
{
    public class OfficeGridViewModel : OfficeBaseViewModel
    {
        [MaxLength(64)]
        public string OfficeTypeName { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
