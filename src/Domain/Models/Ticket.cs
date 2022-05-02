using MGCap.Domain.Entities;
using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    [Table("Tickets")]
    public class Ticket : AuditableCompanyEntity, ISoftDeletable
    {
        /// <summary>
        ///     Number for references purposes
        /// </summary>
        public int Number { get; set; }

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

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

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

        public ICollection<TicketAttachment> Attachments { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Extra 'json' field to store unstructured information
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        public int? FreshdeskTicketId { get; set; }

        /// <summary>
        /// parent ticket identifier, merge purposes
        /// </summary>
        public int? ParentId { get; set; }

        public int? AssignedEmployeeId { get; set; }

        public bool PendingReview { get; set; }

        public bool NewRequesterResponse { get; set; }

        // GMail History Id
        public decimal HistoryId { get; set; }

        // GMail Message Id
        [MaxLength(255)]
        public string MessageId { get; set; } = string.Empty;

        public Ticket()
        {
            this.Attachments = new HashSet<TicketAttachment>();
            this.Data = new Dictionary<string, string>();
        }
    }
}
