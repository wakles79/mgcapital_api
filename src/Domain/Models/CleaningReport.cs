// <copyright file="CleaningReport.cs" company="Axzes">
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
    public class CleaningReport : AuditableCompanyEntity
    {
        /// <summary>
        ///     Gets or sets the current cleaning report Number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Gets or sets the Contact FK
        /// </summary>
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public Contact To { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee From { get; set; }

        [MaxLength(80)]
        public string Location { get; set; }

        public DateTime DateOfService { get; set; }

        public int Submitted { get; set; }

        public ICollection<CleaningReportItem> CleaningReportItems { get; set; }

        public CleaningReport()
        {
            this.CleaningReportItems = new HashSet<CleaningReportItem>();
        }

    }
}
