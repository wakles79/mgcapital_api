// -----------------------------------------------------------------------
// <copyright file="SmtpOptions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  MGCap.Domain.Options
{
    public class SmtpOptions
    {
        public string FromDisplay { get; set; } = string.Empty;

        public string FromEmail { get; set; } = string.Empty;

        public string Server { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public string User { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool UseSsl { get; set; } = false;

        public bool RequiresAuthentication { get; set; } = true;

        public string PreferredEncoding { get; set; } = string.Empty;
    }
}
