// -----------------------------------------------------------------------
// <copyright file="Employee.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    /// <summary>
    /// Represents an Employee of a Company
    /// </summary>
    public class Employee : AuditableCompanyEntity
    {
        [Required]
        [MaxLength(128)]
        public string Email { get; set; }

        public int ContactId { get; set; }

        public int EmployeeStatusId { get; set; }

        public int? DepartmentId { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public bool HasFreshdeskAccount { get; set; }

        public string FreshdeskApiKey { get; set; }

        public string FreshdeskAgentId { get; set; }

        public string EmailSignature { get; set; }
    }
}


