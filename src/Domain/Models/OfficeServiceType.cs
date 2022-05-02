// <copyright file="OfficeServiceType.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class OfficeServiceType : AuditableCompanyEntity
    {
        /// <summary>
        /// Name of office type
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Rate of the office service type
        /// </summary>
        [Required]
        public double Rate { get; set; }

        /// <summary>
        /// Type of rate
        /// <para>0=>Hour, 1=>Amount, 2=>SquareFeet</para>
        /// </summary>
        [Required]
        public ServiceRateType RateType { get; set; }

        /// <summary>
        /// Periodicity of the rate calculation
        /// <para>
        ///	Daily, Monthly, Yearly
        /// </para>
        /// </summary>
        [MaxLength(15)]
        public string Periodicity { get; set; }

        /// <summary>
        /// Status
        /// <para>
        /// false => disabled, true => enabled
        /// </para>
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// SupplyFactor
        /// </summary>	
        public double SupplyFactor { get; set; }
    }
}