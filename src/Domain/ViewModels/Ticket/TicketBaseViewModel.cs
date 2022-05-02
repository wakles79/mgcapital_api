using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketBaseViewModel : AuditableEntityViewModel
    {
        public TicketSource Source { get; set; }

        public TicketStatus Status { get; set; }

        public TicketDestinationType DestinationType { get; set; }

        /// <summary>
        ///     Destination Entity's ID
        /// </summary>
        public int? DestinationEntityId { get; set; }

        public string Description { get; set; }

        [MaxLength(250)]
        public string FullAddress { get; set; }

        public int? BuildingId { get; set; }

        /// <summary>
        ///     Could be an Employee or Customer
        /// </summary>
        public int? UserId { get; set; }

        public UserType UserType { get; set; }

        [MaxLength(200)]
        public string RequesterFullName { get; set; }

        [MaxLength(128)]
        public string RequesterEmail { get; set; }

        [MaxLength(13)]
        public string RequesterPhone { get; set; }

        /// <summary>
        ///     Tickets does not display until snooze day is not the current day
        /// </summary>
        public DateTime? SnoozeDate { get; set; }

        public int EpochSnoozeDate => this.SnoozeDate != null ? this.SnoozeDate.Value.ToEpoch() : 0;

        public ICollection<TicketAttachment> Attachments { get; set; }

        public TicketAdditionalData Data { get; set; }

        public int? FreshdeskTicketId { get; set; }

        public int? ParentId { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public bool PendingReview { get; set; }

        public bool NewRequesterResponse { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class TicketAdditionalData
    {
        public string Location { get; set; }
    }
}
