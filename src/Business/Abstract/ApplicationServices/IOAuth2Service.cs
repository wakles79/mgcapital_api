using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IOAuth2Service
    {
        /// <summary>
        /// Functions that returns the required credentials to work with Google APIs
        /// </summary>
        /// <returns>Google User Credential</returns>
        UserCredential GetUserCredential();
    }
}
