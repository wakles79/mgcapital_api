// <copyright file="ContractOfficeSpace.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ContractOfficeSpace : AuditableEntity
    {
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        public int OfficeTypeId { get; set; }

        [ForeignKey("OfficeTypeId")]
        public OfficeServiceType OfficeServiceType { get; set; } 

        public double SquareFeet { get; set; }
    }
}
