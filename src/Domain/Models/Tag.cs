// -----------------------------------------------------------------------
// <copyright file="Tag.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class Tag : AuditableCompanyEntity
    {
        public string Description { get; set; }

        public TagType Type { get; set; }

        public string HexColor { get; set; }
    }
}
