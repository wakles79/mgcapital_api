using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IToolsApplicationService
    {
        string GoogleStreetViewApiKey();

        AzureStorageConnectionViewModel AzureStorageConnection();
    }
}
