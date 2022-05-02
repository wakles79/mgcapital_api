using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Service
{
    public class ServiceListViewModel : ServiceBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
