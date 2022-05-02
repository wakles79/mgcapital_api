using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Domain.Options
{
    public class AzureStorageOptions
    {
        public string DefaultEndpointsProtocol { get; set; }

        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public string EndpointSuffix { get; set; }

        public string StorageConnectionString => string.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2};EndpointSuffix={3}",
                                                                DefaultEndpointsProtocol, AccountName, AccountKey, EndpointSuffix);

        public string StorageImageBaseUrl { get; set; }
    }
}
