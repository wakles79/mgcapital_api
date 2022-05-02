using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Service
{
    public class ServiceGridViewModel : ServiceBaseViewModel
    {
        public Guid Guid { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
