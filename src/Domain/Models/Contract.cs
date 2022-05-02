// <copyright file="contract.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class Contract : AuditableCompanyEntity
    {
        /// <summary>
        ///     Gets or sets the name of the contract
        /// </summary>
        [MaxLength(128)]
        public string ContractNumber { get; set; }

        /// <summary>
        ///     Gets or sets the area of the contract
        /// </summary>
        public int Area { get; set; }

        /// <summary>
        ///     Gets or sets the number of people involved in the contract
        /// </summary>
        public int NumberOfPeople { get; set; }


        /// <summary>
        ///     Gets or sets the building of the contract
        /// </summary>
        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        /// <summary>
        ///     Gets or sets the Building FK
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        ///     Gets or sets the customer (owner) of the building involved in the contract
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        /// <summary>
        ///     Gets or sets the Customer FK
        /// </summary>
        public int CustomerId { get; set; }


        /// <summary>
        ///     Gets or sets the contact that signed the contract 
        /// </summary>
        [ForeignKey("ContactSignerId")]
        public Contact ContactSigner { get; set; }

        /// <summary>
        ///     Gets or sets the Signer FK
        /// </summary>
        public int ContactSignerId { get; set; }

        /// <summary>
        ///     Gets or sets the contract's status. 
        ///     contract's status could be: '0 - Pending', '1 - Active', '2 - Finished', '3 - Declined'
        /// </summary>
        public int Status { get; set; }


        /// <summary>
        /// Some description of the contract
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Days per month
        /// </summary>
        [Required]
        public double DaysPerMonth { get; set; }

        /// <summary>
        /// Total of restrooms in the contract
        /// </summary>
        public int NumberOfRestrooms { get; set; }

        /// <summary>
        /// Frequency per year
        /// </summary>
        public double FrequencyPerYear { get; set; }

        /// <summary>
        /// Expiration date of the contract
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        public double ProductionRate { get; set; }

        public double UnoccupiedSquareFeets { get; set; }

        /// <summary>
        /// Determine if the contract edition has been completed
        /// </summary>
        public bool EditionCompleted { get; set; }
        
        public double DailyProfit { get; set; }

        public double MonthlyProfit { get; set; }

        public double YearlyProfit { get; set; }

        public double DailyProfitRatio { get; set; }

        public double MonthlyProfitRatio { get; set; }

        public double YearlyProfitRatio { get; set; }

        // TODO: Define remaining fields: CreatedDate, FinishedDate, UpdatedDate, Revenues(Other Entity), Expenses (Other Entity)
    }
}
