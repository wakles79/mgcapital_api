// -----------------------------------------------------------------------
// <copyright file="MessageSender.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MailKit.Net.Smtp;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class MessageSender : IEmailSender, ISmsSender
    {
        private readonly SmtpOptions _smptOptions;
        private readonly EmailSenderOptions _emailSenderOptions;

        public MessageSender(IOptions<SmtpOptions> opts, IOptions<EmailSenderOptions> emailSenderOptions)
        {
            this._smptOptions = opts.Value;
            this._emailSenderOptions = emailSenderOptions.Value;
        }
        /// <inheritdoc/>
        public async Task SendEmailAsync(
            string to,
            string subject,
            string plainTextMessage,
            string htmlMessage = null,
            string fromDisplay = null,
            string replyTo = null,
            IEnumerable<string> cc = null,
            bool isExternal = false)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("No 'To' address provided");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("No 'Subject' provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("No 'Message' provided");
            }

            var msgBody = new MGEmailBody();
            msgBody.IsExternal = isExternal;

            // Gets from display from params
            // otherwise use default from config
            fromDisplay = string.IsNullOrEmpty(fromDisplay) ? this._smptOptions.FromDisplay : fromDisplay;
            msgBody.From.Name = fromDisplay;
            msgBody.From.Email = this._smptOptions.FromEmail;

            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                msgBody.From.Email = replyTo.ReplaceWhitespace();
            }

            var emails = to.Split(',', ';');
            to = emails[0];

            // If there was more than one 'to'
            // we add them to the 'cc' list
            if (emails.Length > 1)
            {
                cc = (cc ?? new List<string>()).Concat(emails.Skip(1));
            }

            msgBody.To.Email = to.ReplaceWhitespace();
            msgBody.Subject = subject;

            // Gets the list of cc from params
            if (cc?.Any() == true)
            {
                foreach (string additionalRecipient in cc)
                {
                    msgBody.Ccs.Add(
                        new MGEmail
                        {
                            Email = additionalRecipient.ReplaceWhitespace()
                        }
                    );
                }
            }

            msgBody.HtmlContent = htmlMessage;
            msgBody.PlainTextContent = plainTextMessage;

            var stringBody = JsonConvert.SerializeObject(msgBody);
#if DEBUG
            // Console.WriteLine("About to send: {0}", stringBody);
#endif

            // Sends POST request to azure function
            using (HttpClient client = new HttpClient())
            {
                var contentType = "application/json";
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var url = this._emailSenderOptions.Url;
                var body = new StringContent(stringBody, Encoding.UTF8, contentType);
                body.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var response = await client.PostAsync(url, body);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // throw new Exception("Unauthorized");
#if DEBUG

                    Console.WriteLine("Email Service returned Unauthorized.");
#endif
                }

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    // Cheers! the request was good.
#if DEBUG

                    Console.WriteLine("Email sent successfully to {0}.", msgBody.To.Email);
#endif
                }
                else
                {
                    // throw new Exception("Bad Request");
#if DEBUG

                    Console.WriteLine("Email Service retured {0}", response.StatusCode);
#endif
                }
            }

        }

        public async Task SendEmailAsyncWithAttachments(
            string to,
            string subject,
            string plainTextMessage,
            string htmlMessage = null,
            string fromDisplay = null,
            string replyTo = null,
            IEnumerable<string> cc = null,
            IDictionary<string, byte[]> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("No 'To' address provided");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("No 'Subject' provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("No 'Message' provided");
            }

            var m = new MimeMessage();

            // Gets from display from params
            // otherwise use default from config
            fromDisplay = string.IsNullOrEmpty(fromDisplay) ? this._smptOptions.FromDisplay : fromDisplay;

            m.From.Add(new MailboxAddress(fromDisplay, this._smptOptions.FromEmail));
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                m.ReplyTo.Add(new MailboxAddress(fromDisplay, replyTo.ReplaceWhitespace()));
            }

            var emails = to.Split(',', ';');
            to = emails[0];

            // If there was more than one 'to'
            // we add them to the 'cc' list
            if (emails.Length > 1)
            {
                cc = (cc ?? new List<string>()).Concat(emails.Skip(1));
            }

            m.To.Add(new MailboxAddress(string.Empty, to.ReplaceWhitespace()));
            m.Subject = subject;

            // Gets the list of cc from params
            if (cc?.Any() == true)
            {
                foreach (string additionalRecipient in cc)
                {
                    m.Cc.Add(new MailboxAddress(string.Empty, additionalRecipient.ReplaceWhitespace()));
                }
            }

            // m.Importance = MessageImportance.Normal;
            // Header h = new Header(HeaderId.Precedence, "Bulk");
            // m.Headers.Add()
            var bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = htmlMessage;
            }

            if (attachments?.Any() == true)
            {
                foreach (var attach in attachments)
                {
                    bodyBuilder.Attachments.Add(attach.Key, attach.Value);
                }
            }

            m.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // TODO: Fix this patch... I know...but well
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                await client.ConnectAsync(this._smptOptions.Server, this._smptOptions.Port, this._smptOptions.UseSsl).ConfigureAwait(false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (this._smptOptions.RequiresAuthentication)
                {
                    await client.AuthenticateAsync(this._smptOptions.User, this._smptOptions.Password).ConfigureAwait(false);
                }

                await client.SendAsync(m).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
