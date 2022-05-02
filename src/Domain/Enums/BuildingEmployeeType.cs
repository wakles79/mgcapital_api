// -----------------------------------------------------------------------
// <copyright file="BuildingEmployeeType.cs" company="Axzes">
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
    [Flags]
    public enum BuildingEmployeeType
    {
        Any = 0,
        Supervisor = 1,
        OperationsManager = 2,
        TemporaryOperationsManager = 4,
        Inspector = 8
    }
}
