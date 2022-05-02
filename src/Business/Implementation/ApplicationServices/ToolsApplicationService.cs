using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Options;
using MGCap.Domain.ViewModels.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ToolsApplicationService : IToolsApplicationService
    {
        private readonly AzureStorageOptions _azureOptions;

        private readonly GoogleStreetViewOptions _gsvOptions;

        public ToolsApplicationService(IOptions<AzureStorageOptions> azureOptions, IOptions<GoogleStreetViewOptions> gsvOptions)
        {
            _azureOptions = azureOptions.Value;
            _gsvOptions = gsvOptions.Value;
        }

        public string GoogleStreetViewApiKey()
        {
            return _gsvOptions.APIKey;
        }

        public AzureStorageConnectionViewModel AzureStorageConnection()
        {
            return new AzureStorageConnectionViewModel
            {
                DefaultEndpointsProtocol = _azureOptions.DefaultEndpointsProtocol,
                AccountName = _azureOptions.AccountName,
                AccountKey = _azureOptions.AccountKey,
                EndpointSuffix = _azureOptions.EndpointSuffix
            };
        }
    }
}
