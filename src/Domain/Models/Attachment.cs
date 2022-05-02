// <copyright file="Attachment.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Domain.Models
{
    public abstract class Attachment : AuditableEntity
    {
        public string Description { get; set; }

        public string BlobName { get; set; }

        public string FullUrl { get; set; }

    }
}
