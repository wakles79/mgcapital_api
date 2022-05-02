using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderWithRequesterViewModel : WorkOrderBaseViewModel
    {
        [MaxLength(250)]
        public string FullAddress { get; set; }

        [MaxLength(200)]
        public string RequesterFullName { get; set; }

        [MaxLength(128)]
        public string RequesterEmail { get; set; }
    }
}
