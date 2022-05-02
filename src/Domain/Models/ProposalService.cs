// -----------------------------------------------------------------------
// <copyright file="ProposalService.cs" company="Axzes">
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
    public class ProposalService : AuditableEntity
    {
        /// <summary>
        /// Foreign key of Proposal
        /// </summary>
        public int ProposalId { get; set; }

        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; }

        /// <summary>
        /// Foreign key of Building
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        /// Name of the building
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// Foreign key of Office Service Type
        /// </summary>
        public int OfficeServiceTypeId { get; set; }

        [ForeignKey("OfficeServiceTypeId")]
        public OfficeServiceType OfficeServiceType { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The requester name
        /// </summary>
        public string RequesterName { get; set; }

        /// <summary>
        /// Some aditional description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Description of the suite or location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Date to be completed
        /// </summary>
        public DateTime? DateToDelivery { get; set; }
    }
}
