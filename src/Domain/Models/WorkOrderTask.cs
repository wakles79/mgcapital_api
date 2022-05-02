// <copyright file="WorkOrderTask.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class WorkOrderTask : AuditableEntity
    {
        public int WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrder { get; set; }

        public bool IsComplete { get; set; }

        public string Description { get; set; }

        public int? ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        public double UnitPrice { get; set; }

        public double Quantity { get; set; }

        public double DiscountPercentage { get; set; }

        public string Note { get; set; }

        /// <summary>
        ///     Determines the last time the task was marked
        ///     as 'completed'
        /// </summary>
        public DateTime LastCheckedDate { get; set; }

        [MaxLength(128)]
        public string Location { get; set; }

        public int? WorkOrderServiceCategoryId { get; set; }

        [ForeignKey("WorkOrderServiceCategoryId")]
        public WorkOrderServiceCategory WorkOrderServiceCategory { get; set; }

        public int? WorkOrderServiceId { get; set; }

        [ForeignKey("WorkOrderServiceId")]
        public WorkOrderService WorkOrderService { get; set; }

        public string UnitFactor { get; set; }

        public WorkOrderServiceFrequency Frequency { get; set; }

        public double Rate { get; set; }

        public double QuantityExecuted { get; set; }

        public double HoursExecuted { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool QuantityRequiredAtClose { get; set; }

        public ICollection<WorkOrderTaskAttachment> Attachments { get; set; }

        public WorkOrderTask()
        {
            this.Attachments = new HashSet<WorkOrderTaskAttachment>();
        }

        public string GeneralNote { get; set; }
    }
}
