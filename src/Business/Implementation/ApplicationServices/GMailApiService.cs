using MGCap.Business.Abstract.ApplicationServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.ViewModels.GMailApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using MGCap.Domain.ViewModels.Freshdesk;
using System.Text.RegularExpressions;
using MGCap.Business.Implementation.ApplicationServices;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CompanySettings;
using System.Web;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class GMailApiService : IGMailApiService
    {
        /// <summary>
        /// Gets <value>IHttpContextAccessor field</value>
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; }

        protected readonly IAzureStorage AzureStorage;

        private readonly ICompanySettingsRepository _CompanySettingsRepository;

        private readonly ITicketAttachmentRepository _TicketAttachmentRepository;

        string ApplicationName = "mgcapital";

        //public string UserEmail
        //{
        //    get
        //    {
        //        return "test@mgcapitalmain.com";
        //    }
        //}

        private string UserEmail { get; }

        private string CompanyGmailEmail { get; }

        public int CompanyId { get; }

        private CompanySettingsDetailViewModel CompanySettings { get; set; }

        private IOptions<GmailApiOptions> GmailApiOptions { get; }

        private int TimezoneOffset;

        private string TimezoneId;

        public GMailApiService(
            IHttpContextAccessor httpContextAccessor,
            ICompanySettingsRepository companySettingsRepository,
            ITicketAttachmentRepository ticketAttachmentRepository,
            IAzureStorage azureStorage,
            IOptions<GmailApiOptions> gmailApiOptions
            )
        {
            this.GmailApiOptions = gmailApiOptions;
            this.HttpContextAccessor = httpContextAccessor;
            string StrCompanyId = this.HttpContextAccessor?.HttpContext?.Request?.Headers["CompanyId"];
            this.CompanyId = string.IsNullOrEmpty(StrCompanyId) ? 1 : int.Parse(StrCompanyId);
            this._CompanySettingsRepository = companySettingsRepository;
            this._TicketAttachmentRepository = ticketAttachmentRepository;
            this.AzureStorage = azureStorage;
            // Get User Email
            this.UserEmail = this.HttpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";
            // Recover default company GmailEmail
            this.CompanySettings = this._CompanySettingsRepository.GetCompanySettingsDapperAsync(this.CompanyId).Result;
            // When null or empty asign default email
            if (this.UserEmail == null)
                this.UserEmail = this.CompanySettings.GmailEmail;
            else if (this.UserEmail == string.Empty || this.UserEmail == "Undefined")
                this.UserEmail = this.CompanySettings.GmailEmail;
            // Verify if email or alias is configured in Gmail Control Panel
            try
            {
                SendAs alias = this.GetService().Users.Settings.SendAs.Get("me", this.UserEmail).Execute();
            }
            catch
            {
                this.UserEmail = this.CompanySettings.GmailEmail;
            }


            var strTimezoneOffset = this.HttpContextAccessor?.HttpContext?.Request?.Headers["TimezoneOffset"];
            this.TimezoneId = this.HttpContextAccessor?.HttpContext?.Request?.Headers["TimezoneId"];

            if (string.IsNullOrEmpty(this.TimezoneId))
            {
                this.TimezoneId = "America/New_York";
            }

            if (string.IsNullOrEmpty(strTimezoneOffset))
            {
                strTimezoneOffset = "300";
            }

            this.TimezoneOffset = (int)Math.Round(double.Parse(strTimezoneOffset));
        }

        public GoogleCredential GetCredential()
        {
            GoogleCredential credential;
            var json = this.GmailApiOptions.Value.ConfigJson;
            credential = GoogleCredential.FromJson(json)
                .CreateScoped(new[] { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend })
                .CreateWithUser(this.CompanySettings.GmailEmail);
            return credential;
        }

        public GmailService GetService()
        {
            GoogleCredential credential = this.GetCredential();
            GmailService service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.ApplicationName
            });
            return service;
        }

        public Message GetMessageContent(string messageId)
        {
            try
            {
                GmailService gmailService = this.GetService();
                UsersResource.MessagesResource.GetRequest message = gmailService.Users.Messages.Get(this.CompanySettings.GmailEmail, messageId);
                Message messageContent = message.Execute();
                return messageContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ConversationBaseViewModel>> GetConversations(string messageId, int ticketId)
        {
            try
            {
                var service = GetService();
                var thread = service.Users.Threads.Get(this.CompanySettings.GmailEmail, messageId).Execute();
                var conversations = new List<ConversationBaseViewModel>();
                foreach (Message msg in thread.Messages)
                {
                    if (msg.Id != messageId)
                    {
                        GMailApiTicketBaseViewModel gmailTicket = await GetEmailData(msg, ticketId);
                        DateTime date = DateTime.UtcNow;

                        string msgDate = (from d in msg.Payload.Headers where d.Name == "Date" select d).First().Value;
                        if (msg.InternalDate != null)
                        {
                            int milliseconds = (int)(msg.InternalDate.Value / 1000);
                            date = DateTimeExtensions.FromEpoch(milliseconds);
                        }
                        else
                        {
                            if (!DateTime.TryParse(msgDate, out date))
                            {
                                date = DateTime.UtcNow;
                            }
                        }
                        conversations.Add(new ConversationBaseViewModel
                        {
                            CreatedAt = date,
                            SupportEmail = gmailTicket.From,
                            Body = gmailTicket.Body,
                            BodyText = gmailTicket.BodyText,
                            Attachments = await this.GetGmailAttachments(msg.Id, ticketId)
                        });
                    }
                }
                IEnumerable<ConversationBaseViewModel> conv = conversations;
                return conv;
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<IEnumerable<ConversationBaseViewModel>>();
                tcs.SetResult(new List<ConversationBaseViewModel>());
                return tcs.Task.Result;
            }
        }

        private async Task<IEnumerable<AttachmentsBaseViewModel>> GetGmailAttachments(string messageId, int? ticketId = null)
        {
            GmailService service = this.GetService();
            //string userId = this.UserEmail;
            List<AttachmentsBaseViewModel> result = new List<AttachmentsBaseViewModel>();
            try
            {
                Message message = service.Users.Messages.Get(this.CompanySettings.GmailEmail, messageId).Execute();
                if (message.Payload.Parts != null)
                {
                    IList<MessagePart> parts = this.GetPartsRecursively(message.Payload.Parts);
                    foreach (MessagePart part in parts)
                    {
                        if (!string.IsNullOrEmpty(part.Filename))
                        {
                            var data = "";
                            if (!string.IsNullOrEmpty(part.Body.Data))
                            {
                                data = part.Body.Data;
                            }
                            else
                            {
                                var attId = part.Body.AttachmentId;
                                var attachPart = service.Users.Messages.Attachments.Get(this.CompanySettings.GmailEmail, messageId, attId).Execute();
                                data = attachPart.Data;
                            }
                            var gmailId = part.Headers?.FirstOrDefault(p => p.Name == "Content-ID")?.Value ?? string.Empty;
                            var attc = new AttachmentsBaseViewModel
                            {
                                Id = 0,
                                GmailId = gmailId,
                                ContentType = part.MimeType
                            };
                            // Checks if there is any attachment with the same gmailId 
                            var candidate = await this._TicketAttachmentRepository.GetTicketAttachmentByGmailIdAsync(gmailId);
                            if (candidate != null && !string.IsNullOrEmpty(candidate.FullUrl))
                            {
                                attc.Name = candidate.BlobName;
                                attc.AttachmentUrl = candidate.FullUrl;
                            }
                            else
                            {
                                // If there is no attachment with the same gmailId, then create a new one
                                // Converting from RFC 4648 base64 to base64url encoding
                                // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                                var attachData = data.Replace('-', '+');
                                attachData = attachData.Replace('_', '/');

                                var filebytes = Convert.FromBase64String(attachData);
                                var ms = new MemoryStream(filebytes);
                                var uploadedFile = new Tuple<string, string, DateTime>(string.Empty, string.Empty, DateTime.UtcNow);
                                var ext = Path.GetExtension(part.Filename).Replace(".", "");
                                var mime = part.MimeType;
                                // checks if the file is an image
                                if (mime.Contains("image"))
                                {
                                    uploadedFile = await this.AzureStorage.UploadImageAsync(ms, ext: ext, contentType: part.MimeType);
                                }
                                else
                                {
                                    uploadedFile = await this.AzureStorage.UploadFileAsync(ms, ext, part.MimeType);
                                }

                                attc.Name = uploadedFile.Item1;
                                attc.AttachmentUrl = uploadedFile.Item2;
                                if (ticketId != null)
                                {
                                    await this._TicketAttachmentRepository.AddDapperAsync(new TicketAttachment
                                    {
                                        CreatedBy = this.UserEmail,
                                        UpdatedBy = this.UserEmail,
                                        CreatedDate = DateTime.UtcNow,
                                        UpdatedDate = DateTime.UtcNow,
                                        TicketId = ticketId.Value,
                                        BlobName = uploadedFile.Item1,
                                        FullUrl = uploadedFile.Item2,
                                        GmailId = gmailId,
                                    });
                                }
                            }

                            //GMailEmailAttachment attc = new GMailEmailAttachment() { FileName = part.Filename, Azure_UploadId = 11, Url = "" };
                            result.Add(attc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            IEnumerable<AttachmentsBaseViewModel> attachments = result;
            return attachments;
        }

        private IList<MessagePart> GetPartsRecursively(IList<MessagePart> parts)
        {
            var results = new List<MessagePart>(parts);
            foreach (MessagePart part in parts)
            {
                if (part.Parts != null)
                {
                    results.AddRange(this.GetPartsRecursively(part.Parts));
                }
            }

            return results;
        }

        public List<GMailEmailAttachment> GetAttachments(string messageId, string outputDir)
        {
            GmailService service = this.GetService();
            //string userId = this.UserEmail;
            List<GMailEmailAttachment> result = new List<GMailEmailAttachment>();
            try
            {
                Message message = service.Users.Messages.Get(this.CompanySettings.GmailEmail, messageId).Execute();
                IList<MessagePart> parts = message.Payload.Parts;
                foreach (MessagePart part in parts)
                {
                    if (!String.IsNullOrEmpty(part.Filename))
                    {
                        string data = "";
                        if (!String.IsNullOrEmpty(part.Body.Data))
                        {
                            data = part.Body.Data;
                        }
                        else
                        {
                            String attId = part.Body.AttachmentId;
                            MessagePartBody attachPart = service.Users.Messages.Attachments.Get(this.CompanySettings.GmailEmail, messageId, attId).Execute();
                            data = attachPart.Data;
                        }


                        // Converting from RFC 4648 base64 to base64url encoding
                        // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                        String attachData = data.Replace('-', '+');
                        attachData = attachData.Replace('_', '/');

                        byte[] filebytes = Convert.FromBase64String(attachData);
                        File.WriteAllBytes(Path.Combine(outputDir, part.Filename), filebytes);

                        String mime = part.MimeType;

                        GMailEmailAttachment attc = new GMailEmailAttachment() { FileName = part.Filename, Azure_UploadId = 11, Url = "" };
                        result.Add(attc);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return result;
        }

        public Message SendEmail(string to, string from, string subject, string bodyText, bool IsHtml, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            GmailService service = GetService();
            var mailMessage = new MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(from);
            mailMessage.To.Add(to);
            if (cc != null)
                foreach (string mailAddress in cc)
                {
                    mailMessage.CC.Add(mailAddress);
                }
            if (bcc != null)
                foreach (string bcAddress in bcc)
                {
                    mailMessage.Bcc.Add(bcAddress);
                }
            mailMessage.ReplyToList.Add(from);
            mailMessage.Subject = subject;

            // var alias = service.Users.Settings.SendAs.Get("me", this.UserEmail).Execute();
            // try
            // {
            //     alias = service.Users.Settings.SendAs.Get(this.UserEmail, this.UserEmail).Execute();
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e.Message);
            // }
            // string signature = alias.Signature;
            // if (!string.IsNullOrEmpty(signature))
            // {
            //     int divSpaces = 5;
            //     int index = bodyText.IndexOf("class=\"quoted-text\"");
            //     index -= divSpaces;
            //     if (index >= 0)
            //     {
            //         bodyText = bodyText.Insert(index, $"<br>{signature}<br>");
            //     }
            //     else
            //     {
            //         bodyText += $"<br>{signature}";
            //     }
            // }
            mailMessage.Body = bodyText;
            mailMessage.IsBodyHtml = true;

            //foreach (System.Net.Mail.Attachment attachment in email.Attachments)
            //{
            //    mailMessage.Attachments.Add(attachment);
            //}

            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

            var gmailMessage = new Message
            {
                Raw = Base64UrlEncode(mimeMessage.ToString())
            };

            UsersResource.MessagesResource.SendRequest request = service.Users.Messages.Send(gmailMessage, this.CompanySettings.GmailEmail);

            Message send = request.Execute();
            return send;
        }

        public async Task<Message> ReplyEmail(Message replayMSG, FreshdeskTicketReplyAttachedFilesViewModel attachedBody, int ticketId)
        {
            var service = GetService();
            var data = await GetEmailData(replayMSG, ticketId);
            var reply = JsonConvert.DeserializeObject<TicketReplyViewModel>(attachedBody.ReplyModel);
            var body = reply.Body;// HttpUtility.HtmlEncode(data.Body);
            var mailMessage = new MailMessage();
            var replyTo = data.ReplyTo;
            if (string.IsNullOrEmpty(replyTo))
            {
                replyTo = data.From;
            }
            mailMessage.From = new System.Net.Mail.MailAddress(data.To);
            mailMessage.To.Add(replyTo);
            mailMessage.ReplyToList.Add(data.To);
            mailMessage.Subject = data.Subject;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

            // UserCredential Default Signature
            var alias = service
                        .Users.Settings.SendAs.Get(this.CompanySettings.GmailEmail, this.UserEmail)
                        .Execute();

            // var signature = alias.Signature;
            // if (!string.IsNullOrEmpty(signature))
            // {
            //     body = $"{body}<br>{signature}";
            // }

            // Quote Original email
            // To form the body, first we need to recover the thread
            var thread = service
                            .Users.Threads.Get(this.CompanySettings.GmailEmail, replayMSG.ThreadId)
                            .Execute();
            // 2nd we will need the last message from this thread sent by the CLient
            var lastMsg = thread.Messages.Last();
            var lastMsgData = await this.GetEmailData(lastMsg, ticketId);
            foreach (var item in thread.Messages)
            {
                var temp = await this.GetEmailData(item, ticketId);
                if (temp != null && temp.From != null && !temp.From.Contains(this.CompanySettings.GmailEmail))
                {
                    lastMsg = item;
                    lastMsgData = temp;
                }
            }

            // 3rd Recover and decode last message full body
            var epoch = Convert.ToInt32((lastMsg.InternalDate ?? 0) / 1000);
            var date = epoch.AdjustDateTime(this.TimezoneOffset, this.TimezoneId);
            var formattedDate = string.Format("{0:ddd, MMM dd, yyyy a't' hh:mm tt}", date);
            var replyBody = lastMsgData.Body;

            // 4th Use the Template and add the raw response and rawlastmessage body

            mailMessage.Body = $@"
            <div class=""gmail_default"">{body}</div>
            <div dir=""ltr"" class=""gmail_attr"">On {formattedDate} &lt;<a href=""mailto:{data.From}"" target=""_blank"">{data.From}</a>&gt; wrote:<br></div>
            <div data-thread class=""gmail_quote"" style=""margin:0px 0px 0px 0.8ex;border-left:1px solid rgb(204,204,204);padding-left:1ex"">{replyBody}</div>";

            mailMessage.Body = this.EncodeSpecialCharater(mailMessage.Body);

            mailMessage.IsBodyHtml = true;

            if (attachedBody.Attachments != null)
            {
                foreach (var attachment in attachedBody.Attachments)
                {
                    if (attachment.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            attachment.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            var att = new System.Net.Mail.Attachment(new MemoryStream(fileBytes), attachment.FileName);
                            mailMessage.Attachments.Add(att);
                        }
                    }
                }
            }

            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);
            var references = lastMsgData.References;
            var parentId = lastMsgData.InReplyTo ?? lastMsgData.HeaderMessageID;
            var h1 = new MimeKit.Header("References", $"{references} {parentId}");
            var h2 = new MimeKit.Header("In-Reply-To", parentId);
            mimeMessage.Headers.Add(h1);
            mimeMessage.Headers.Add(h2);

            var gmailMessage = new Message
            {
                Raw = Base64UrlEncode(mimeMessage.ToString()),
                ThreadId = lastMsg.ThreadId,
            };

            var request = service.Users.Messages.Send(gmailMessage, alias.SendAsEmail);

            var send = request.Execute();
            return send;
        }

        public Message GetMessagesById(GmailService gmailService, string userEmail, string MessagesId)
        {
            try
            {
                var emailListRequest = gmailService.Users.Messages.Get(this.CompanySettings.GmailEmail, MessagesId);
                var msg = emailListRequest.Execute();
                return msg;
            }
            catch (Exception)
            {

            }
            return null;

        }

        public void GetThread(GmailService service)
        {
            List<Thread> result = new List<Thread>();
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List(this.CompanySettings.GmailEmail);
            //request.LabelIds = labelIds;
            //request.Q = query;
            do
            {
                try
                {
                    ListThreadsResponse response = request.Execute();
                    result.AddRange(response.Threads);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured: " + e.Message);
                }

            } while (!String.IsNullOrEmpty(request.PageToken));
        }

        public async Task<GMailApiTicketBaseViewModel> GetEmailData(Message msg, int? ticketId = null)
        {
            var email = new GMailApiTicketBaseViewModel();
            if (msg == null || msg.Payload == null)
            {
                return email;
            }
            // Loop through the headers and get the fields we need...
            foreach (var mParts in msg.Payload.Headers)
            {
                switch (mParts.Name)
                {
                    case "Date":
                        email.Date = mParts.Value;
                        break;
                    case "From":
                        email.From = mParts.Value;
                        break;
                    case "Subject":
                        email.Subject = mParts.Value;
                        break;
                    case "Delivered-To":
                        email.DeliveredTo = mParts.Value;
                        break;
                    case "To":
                        email.To = mParts.Value;
                        break;
                    case "References":
                        email.References = mParts.Value;
                        break;
                    case "In-Reply-To":
                        email.InReplyTo = mParts.Value;
                        break;
                    case "Reply-To":
                        email.ReplyTo = ExtractEmailFromText(mParts.Value);
                        break;
                    case "Message-ID":
                    case "Message-Id":
                        email.HeaderMessageID = mParts.Value;
                        break;
                    case "Thread-Topic":
                        email.ThreadTopic = mParts.Value;
                        break;
                    default:
                        break;
                }
            }

            //get Name From Email From
            // Recover Message Name
            string[] words = email.From.Split(' ');
            foreach (var word in words)
            {
                if (word.Trim() != "")
                {
                    string t = word.Substring(0, 1);
                    if (word.Substring(0, 1) != "\u003c")
                    {
                        if (email.FromName == "")
                        {
                            email.FromName = word;
                        }
                        else
                        {
                            email.FromName += " " + word;
                        }
                    }
                }
            }

            email.From = ExtractEmailFromText(email.From);
            if (string.IsNullOrEmpty(email.FromName))
            {
                email.FromName = email.From;
            }

            if (msg.Payload.Parts == null && msg.Payload.Body != null)
            {
                var sBody = this.Base64Decode(msg.Payload.Body.Data);
                email.Body = sBody;
            }
            else
            {
                email.Body = GetNestedPartsHtml(msg.Payload.Parts, "");
            }

            email.Attachments = await this.GetGmailAttachments(msg.Id, ticketId);
            foreach (var att in email.Attachments)
            {
                if (!att.ContentType.Contains("image"))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(att.GmailId))
                {
                    var gmailId = att.GmailId?.Replace("<", "")?.Replace(">", "");
                    email.Body = email.Body.Replace("cid:" + gmailId, att.AttachmentUrl);
                }
            }

            email.BodyText = email.Body;
            return email;
        }

        public string ExtractEmailFromText(string text)
        {
            const string MatchEmailPattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
              + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
              + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";

            Regex rx = new Regex(
              MatchEmailPattern,
              RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            MatchCollection matches = rx.Matches(text);

            // Report the number of matches found.
            int noOfMatches = matches.Count;

            if (noOfMatches == 1)
            {
                return matches[0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }

        string GetNestedParts(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if (part == null)
            {
                return str;
            }
            else
            {
                foreach (var parts in part)
                {
                    if (parts.Parts == null)
                    {
                        if (parts.Body != null && parts.Body.Data != null)
                        {
                            String sBody = parts.Body.Data.Replace("-", "+");
                            sBody = sBody.Replace("_", "/");
                            byte[] data = Convert.FromBase64String(sBody);
                            string bodyPard = Encoding.UTF8.GetString(data);

                            str += bodyPard;
                        }
                    }
                    else
                    {
                        return GetNestedParts(parts.Parts, str);
                    }
                }

                return str;
            }
        }

        string GetNestedPartsText(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if (part == null)
            {
                return str;
            }
            else
            {

                foreach (var parts in part)
                {
                    if (parts.Parts == null)
                    {
                        if (parts.MimeType == "text/plain")
                            if (parts.Body != null && parts.Body.Data != null)
                            {
                                String sBody = parts.Body.Data.Replace("-", "+");
                                sBody = sBody.Replace("_", "/");
                                byte[] data = Convert.FromBase64String(sBody);
                                string bodyPard = Encoding.UTF8.GetString(data);

                                str += bodyPard;
                            }
                    }
                    else
                    {
                        return GetNestedPartsText(parts.Parts, str);
                    }

                    /*if (parts.MimeType == "text/plain")
                        if (parts.Parts == null)
                        {
                            if (parts.Body != null && parts.Body.Data != null)
                            {
                                String sBody = parts.Body.Data.Replace("-", "+");
                                sBody = sBody.Replace("_", "/");
                                byte[] data = Convert.FromBase64String(sBody);
                                string bodyPard = Encoding.UTF8.GetString(data);

                                str += bodyPard;
                            }
                        }
                        else
                        {
                            return str += getNestedPartsText(parts.Parts, str);
                        }
                    else
                    {
                        str += getNestedParts(parts.Parts, "");
                    }*/
                }

                return str;
            }
        }

        string GetNestedPartsHtml(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if (part == null)
            {
                return str;
            }

            foreach (var parts in part)
            {
                if (parts.Parts != null)
                {
                    return GetNestedPartsHtml(parts.Parts, str);
                }
                if (parts.MimeType == "text/html")
                {
                    if (parts.Body != null && parts.Body.Data != null)
                    {
                        string bodyPard = Base64Decode(parts.Body.Data);
                        str += bodyPard;
                    }
                }
            }

            return str;
        }

        public string Base64Decode(string base64Text)
        {
            var cleanValue = base64Text
              .Replace('-', '+')
              .Replace('_', '/');
            var valueBytes = System.Convert.FromBase64String(cleanValue);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public string Base64ToByte(string base64Text)
        {
            var inputBytes = Encoding.UTF8.GetBytes(base64Text);
            return Convert.ToBase64String(inputBytes);
        }

        public void MarkAsRead(string hostEmailAddress, string messageId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AddFromTicket(Ticket ticket)
        {
            try
            {
                #region GMail
                string to = ticket.RequesterEmail;
                string from = this.CompanySettings.GmailEmail; // Maybe it must be this.UserEmail?
                string subject = ticket.Description.Length > 25 ? $"{ticket.Description.Substring(0, 25)}..." : ticket.Description;
                string bodyText = ticket.Description;
                bool isHtml = true;
                var msgResponse = SendEmail(to, from, subject, bodyText, isHtml, null, null);
                return msgResponse.Id;
                #endregion GMail
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        async Task<FreshdeskTicketBaseViewModel> ConvertGmailMessageToFreshDesk(Message message)
        {
            var gmailTicket = await GetEmailData(message);
            // Still waiting to edit this part
            var fdTicket = new FreshdeskTicketBaseViewModel
            {
                CreatedAt = DateTime.Parse(gmailTicket.Date),
                Email = gmailTicket.From,
                Subject = gmailTicket.Subject,
                Description = gmailTicket.Body,
                DescriptionText = gmailTicket.BodyText,
                MessageId = gmailTicket.HeaderMessageID
            };
            return fdTicket;
        }

        public async Task<ListHistoryResponse> GetHistory(ulong historyId)
        {
            var service = GetService();
            var history = service.Users.History.List(this.CompanySettings.GmailEmail);
            history.StartHistoryId = historyId;
            return history.Execute();
        }

        // Runs Watch Gmail API command to start push notifications
        public ulong? StartService()
        {
            // Get Service
            var service = this.GetService();
            // Get Gmail Email
            var companySettings = this._CompanySettingsRepository.SingleOrDefault(c => c.ID == CompanyId);
            // Create Request Body
            var body = new WatchRequest
            {
                TopicName = this.GmailApiOptions.Value.TopicName,
                LabelIds = new[] { "INBOX" }
            };
            // Run Watch and recover response
            var response = service.Users.Watch(body, companySettings.GmailEmail).Execute();
            // Recover HistoryId
            ulong? historyId = response.HistoryId;
            // Save HistoryId
            return historyId;
        }

        // Runs Stop Gmail API command to stop push notifications
        public void StopService()
        {
            // Get Service
            var service = this.GetService();
            // Get Gmail Email
            var companySettings = this._CompanySettingsRepository.SingleOrDefault(c => c.ID == CompanyId);
            // Run Stop Command
            service.Users.Stop(companySettings.GmailEmail).Execute();
        }

        public string EncodeSpecialCharater(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string output = "";

            foreach (var c in input.ToArray())
            {
                if (c < 127)
                {
                    output = output + c;
                }
                else
                {
                    output = output + HttpUtility.HtmlEncode("" + c);
                }
            }

            return output;
        }
    }
}
