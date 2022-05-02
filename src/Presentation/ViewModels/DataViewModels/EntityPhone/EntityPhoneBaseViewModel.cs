using MGCap.Presentation.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.EntityPhone
{
    public class EntityPhoneBaseViewModel : EntityViewModel
    {
        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public int EntityId { get; set; }

        public string Type { get; set; }

        public bool Default { get; set; }
    }
}
