// -----------------------------------------------------------------------
// <copyright file="ContactType.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    public enum ContactType
    {
        /// <summary>
        /// Customer Contact
        /// </summary>
        Customer,

        /// <summary>
        /// Vendor Contact
        /// </summary>
        Vendor,

        /// <summary>
        /// Building Contact
        /// </summary>
        Building,

        Undefined,
    }
}
