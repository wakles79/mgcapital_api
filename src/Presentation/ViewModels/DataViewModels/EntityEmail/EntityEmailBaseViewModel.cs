using MGCap.Presentation.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.EntityEmail
{
    public class EntityEmailBaseViewModel : EntityViewModel

    {
        public int EntityId { get; set; }

        [MaxLength(128)]
        public string Email { get; set; }

        public string Type { get; set; }

        public bool Default { get; set; }

    }
}
