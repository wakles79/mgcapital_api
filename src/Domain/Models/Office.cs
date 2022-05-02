// <copyright file="Office.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class Office : AuditableCompanyEntity
    {
        /// <summary>
        /// Gets of sets the Name of the office
        /// </summary>
        [MaxLength(128)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of the office
        /// </summary>
        [MaxLength(128)]
        public string Description { get; set; }

        /// <summary>
        /// Indicates the location of the office on the building
        /// </summary>
        /// [MaxLength(128)]
        public string Location { get; set; }

        /// <summary>
        /// Width of the office
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height of the office
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Total square feet of the office
        /// </summary>
        public double SquareFeet { get; set; }

        /// <summary>
        /// Estate of the office
        /// Values: 'false = disabled' or 'true = enabled'
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Id of the associated building
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        /// Foreign key id of the office type
        /// </summary>
        public int OfficeTypeId { get; set; }

        /// <summary>
        /// Type of the office 
        /// </summary>
        [ForeignKey("OfficeTypeId")]
        public OfficeServiceType OfficeType { get; set; }
    }
}
