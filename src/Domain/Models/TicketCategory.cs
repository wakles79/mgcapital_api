using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    public class TicketCategory : AuditableCompanyEntity
    {
        public string Name { get; set; }
    }
}
