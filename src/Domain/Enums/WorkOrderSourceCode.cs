// -----------------------------------------------------------------------
// <copyright file="WorkOrderSourceCode.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Domain.Enums
{
    public enum WorkOrderSourceCode
    {
        Email = 0,

        Phone = 1,

        InPerson = 2,

        Recurring = 3,

        InternalMobile = 10,

        ExternalMobile = 11,

        LandingPage = 12,

        Calendar = 13,

        Ticket = 14,

        Other = 16
    }
}
