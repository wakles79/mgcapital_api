using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Domain.ViewModels.Employee
{
    public class EmployeeBaseViewModel : EntityViewModel
    {
        [MaxLength(128)]
        public string Email { get; set; }

        public int EmployeeStatusId { get; set; }

        public int? DepartmentId { get; set; }

        public int ContactId { get; set; }

        public bool HasFreshdeskAccount { get; set; }

        public string FreshdeskApiKey { get; set; }

        public string FreshdeskAgentId { get; set; }

        public string EmailSignature { get; set; }
    }
}
