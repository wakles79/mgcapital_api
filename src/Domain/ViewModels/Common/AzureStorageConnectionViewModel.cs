using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Common
{
    public class AzureStorageConnectionViewModel
    {
        public string DefaultEndpointsProtocol { get; set; }

        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public string EndpointSuffix { get; set; }
    }
}
