// -----------------------------------------------------------------------
// <copyright file="Inspection.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Inspection : AuditableCompanyEntity
    {
        public int Number { get; set; }

        public DateTime? SnoozeDate { get; set; }

        public int BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        public int EmployeeId { get; set; }

        public int Stars { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public string BeginNotes { get; set; }

        public string ClosingNotes { get; set; }

        public int Score { get; set; }

        public bool AllowPublicView { get; set; }

        public InspectionStatus Status { get; set; }

        public ICollection<InspectionItem> Items { get; set; }

        public ICollection<InspectionNote> Notes { get; set; }

        public Inspection()
        {
            this.Items = new HashSet<InspectionItem>();
            this.Notes = new HashSet<InspectionNote>();
        }
    }
}
