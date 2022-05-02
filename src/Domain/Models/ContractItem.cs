// <copyright file="ContractItem.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class ContractItem : AuditableEntity
    {
        /// <summary>
        /// Quantity of items
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Description of the item
        /// </summary>
        [MaxLength(128)]
        public string Description { get; set; }

        /// <summary>
        /// Foreign key of the contract
        /// </summary>
        [ForeignKey("ContractId")]
        public int ContractId { get; set; }

        /// <summary>
        /// Office service type FK
        /// </summary>
        [ForeignKey("OfficeServiceTypeId")]
        public OfficeServiceType OfficeServiceType { get; set; }

        /// <summary>
        /// Id of the type of office service
        /// </summary>
        public int OfficeServiceTypeId { get; set; }

        /// <summary>
        /// Copy of the name of the office service type
        /// </summary>
        public string OfficeServiceTypeName { get; set; }

        /// <summary>
        /// Copy of the rate of the office service type
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Copy of the type of rate of the type of office service
        /// </summary>
        public ServiceRateType RateType { get; set; }

        /// <summary>
        /// Copy of the periodicity of the type of office service
        /// </summary>
        [MaxLength(15)]
        public string RatePeriodicity { get; set; }

        /// <summary>
        /// Total of hours of the item, according to the type of office service
        /// </summary>
        public double? Hours { get; set; }

        /// <summary>
        /// Total amount of the item, according to the type of office service
        /// </summary>
        public double? Amount { get; set; }

        /// <summary>
        /// Total of rooms of the item, according to the type of office service
        /// </summary>
        public double? Rooms { get; set; }

        /// <summary>
        /// Total square feet of the item, according to the type of office service
        /// </summary>
        public double? SquareFeet { get; set; }

        public ContractItemDefaultType DefaultType { get; set; }

        public double DailyRate { get; set; }

        public double MonthlyRate { get; set; }

        public double YearlyRate { get; set; }

        public int Order { get; set; }
    }
}