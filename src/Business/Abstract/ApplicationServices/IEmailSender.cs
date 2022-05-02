// -----------------------------------------------------------------------
// <copyright file="IEmailSender.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(
            string to,
            string subject,
            string plainTextMessage,
            string htmlMessage = null,
            string fromDisplay = null,
            string replyTo = null,
            IEnumerable<string> cc = null,
            bool isExternal = false);

        Task SendEmailAsyncWithAttachments(
            string to,
            string subject,
            string plainTextMessage,
            string htmlMessage = null,
            string fromDisplay = null,
            string replyTo = null,
            IEnumerable<string> cc = null,
            IDictionary<string, byte[]> attachments = null);
    }
}
