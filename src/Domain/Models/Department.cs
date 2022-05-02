using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Department : AuditableCompanyEntity
    {
        [MaxLength(80)]
        public string Name { get; set; }
    }
}
