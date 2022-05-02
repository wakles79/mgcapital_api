// -----------------------------------------------------------------------
// <copyright file="TicketTag.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class TicketEmailHistory : Entity
    {
        public decimal HistoryId { get; set; }
        [MaxLength(255)]
        public string ThreadId { get; set; }
        [MaxLength(255)]
        public string MessageId { get; set; }
        public string RawMessage { get; set; }
        public decimal Timestamp { get; set; }

    }
}
