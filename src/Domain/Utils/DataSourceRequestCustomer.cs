using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestCustomer : DataSourceRequest
    {
        /// <summary>
        ///     A value indicating if all customer must have at least one contact
        /// </summary>
        public int? WithContacts { get; set; }
    }
}
