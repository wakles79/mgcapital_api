// <copyright file="Building.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class Building : AuditableCompanyEntity
    {
        /// <summary>
        ///     Gets or sets the name of the building
        /// </summary>
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Address FK
        /// </summary>
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(15)]
        public string EmergencyPhone { get; set; }

        [MaxLength(10)]
        public string EmergencyPhoneExt { get; set; }

        [MaxLength(128)]
        public string EmergencyNotes { get; set; }

        [MaxLength(32)]
        [Required]
        public string Code { get; set; }

        /// <summary>
        ///     Gets or sets building employees, at this moment these can be supervisors or operations manager
        /// </summary>
        public ICollection<BuildingEmployee> Employees { get; set; }

        public ICollection<BuildingContact> Contacts { get; set; }

        public int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public Building()
        {
            this.Employees = new HashSet<BuildingEmployee>();
            this.Contacts = new HashSet<BuildingContact>();
        }
    }
}
