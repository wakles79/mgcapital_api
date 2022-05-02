using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class OneSignalException : Exception
    {
        public OneSignalException(Exception ex) : base("Error while sending push notification", ex)
        {

        }
    }
}
