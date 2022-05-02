// -----------------------------------------------------------------------
// <copyright file="WorkOrderType.cs" company="Axzes">
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
    public enum WorkOrderType
    {
        Request,

        Complaint,

        Important,

        Other,

        Specialist,

        Observation
    }
}
