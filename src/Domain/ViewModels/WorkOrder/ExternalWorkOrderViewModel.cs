using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class ExternalWorkOrderViewModel
    {
        [MaxLength(128)]
        [Required]
        public string RequesterEmail { get; set; }

        [MaxLength(13)]
        public string RequesterPhone { get; set; }

        public string Description { get; set; }

        [MaxLength(200)]
        public string RequesterFullName { get; set; }

        [MaxLength(250)]
        public string FullAddress { get; set; }

        [MaxLength(128)]
        public string Location { get; set; }
    }
}
