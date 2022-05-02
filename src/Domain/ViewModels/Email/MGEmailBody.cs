using System.Collections.Generic;

namespace MGCap.Domain.ViewModels.Email
{
    public class MGEmail
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class MGEmailBody
    {
        public string Subject { get; set; }
        public MGEmail To { get; set; } = new MGEmail { };

        public MGEmail From { get; set; } = new MGEmail
        {
            Email = "customerservice@mgcapitalmain.com",
            Name = "MG Capital Maintenance"
        };

        public List<MGEmail> Ccs { get; set; } = new List<MGEmail>();

        public string PlainTextContent { get; set; }

        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whenever the contact
        /// is external (customer, external) or internal (employee)
        /// </summary>
        /// <value></value>
        public bool IsExternal { get; set; }

    }
}