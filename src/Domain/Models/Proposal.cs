// -----------------------------------------------------------------------
// <copyright file="Proposal.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Proposal : AuditableCompanyEntity
    {
        /// <summary>
        /// The client that receives the proposal
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Name of the customer
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Email of the customer
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// The user who created the proposal
        /// </summary>
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

        /// <summary>
        /// Description of the location
        /// </summary>
        [MaxLength(128)]
        public string Location { get; set; }

        /// <summary>
        /// Can be '0: Pending', '1: Approved', '2: Declined'
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// The date when the proposal was declined or approved
        /// </summary>
        public DateTime? StatusChangedDate { get; set; }

        /// <summary>
        /// To whom the proposal should be billed.
        /// can be 1 => tenant, 2 Management company
        /// </summary>
        public int? BillTo { get; set; }

        /// <summary>
        /// Name of whom will be billed
        /// </summary>
        public string BillToName { get; set; }

        /// <summary>
        /// Email of whom will be billed
        /// </summary>
        public string BillToEmail { get; set; }
    }
}
