using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class WorkOrderFromMailApplicationService : IWorkOrderFromMailApplicationService
    {
        private readonly IWorkOrdersRepository _workOrdersRepository;

        public WorkOrderFromMailApplicationService(IWorkOrdersRepository workOrdersRepository)
        {
            _workOrdersRepository = workOrdersRepository;
        }

        public async Task<int> ReadInbox()
        {
            try
            {
                UserCredential credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "682013489512-i28g1jkomsbpb5eq5ajd852b37a7h7k4.apps.googleusercontent.com",
                        ClientSecret = "Cmj1wiYMEP9H91qWhpsL4l0Q"
                    },
                    new[] { GmailService.Scope.MailGoogleCom },
                    "mgworkorder@gmail.com",
                    CancellationToken.None);

                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = "MGCapital"
                });

                List<Message> emails = new List<Message>();
                UsersResource.MessagesResource.ListRequest listRequest = service.Users.Messages.List("me");
                listRequest.IncludeSpamTrash = false;
                listRequest.Q = "-label:mgcapital_read";

                ListLabelsResponse responseLabels = service.Users.Labels.List("me").Execute();
                List<Label> labels = new List<Label>(responseLabels.Labels);
                List<string> labelIds = labels.Where(l => l.Name.Equals("MGCAPITAL_READ")).Select(l => l.Id).ToList();

                do
                {
                    ListMessagesResponse response = listRequest.Execute();
                    IEnumerable<Message> range = response.Messages.Where(m => IsUnread(m, labelIds.First()));

                    if (range?.Count() > 0)
                    {
                        emails.AddRange(range);
                        listRequest.PageToken = response.NextPageToken;
                    }
                    else
                    {
                        listRequest.PageToken = string.Empty;
                    }
                }
                while (string.IsNullOrEmpty(listRequest.PageToken) == false);

                BatchModifyMessagesRequest body = new BatchModifyMessagesRequest
                {
                    AddLabelIds = labelIds,
                    Ids = emails?.Select(m => m.Id)?.ToList() ?? new List<string>()
                };

                if (body.Ids.Count > 0)
                {
                    UsersResource.MessagesResource.BatchModifyRequest batchModifyRequest = service.Users.Messages.BatchModify(body, "me");
                    batchModifyRequest.Execute();
                }

                List<WOFromEMailViewModel> requestedWorkOrders = new List<WOFromEMailViewModel>();
                UsersResource.MessagesResource.GetRequest getRequest;

                foreach (var messageIter in emails)
                {
                    getRequest = service.Users.Messages.Get("me", messageIter.Id);
                    Message message = getRequest.Execute();
                    WOFromEMailViewModel woFromEMail = new WOFromEMailViewModel();

                    string senderInfo = message.Payload.Headers.First(h => h.Name.ToLowerInvariant().Equals("from"))?.Value;
                    string[] info = senderInfo.Split('<');

                    if (info.Length > 1)
                    {
                        woFromEMail.RequesterName = info[0]?.Trim();
                        woFromEMail.RequesterEmail = info[1].Replace(">", "");
                    }
                    else
                    {
                        woFromEMail.RequesterEmail = senderInfo.ToLowerInvariant();
                    }

                    woFromEMail.Subject = message.Payload.Headers.First(h => h.Name.ToLowerInvariant().Equals("subject"))?.Value;
                    woFromEMail.ReceivedDate = message.Payload.Headers.First(h => h.Name.ToLowerInvariant().Equals("date"))?.Value;

                    string text = string.Empty;

                    if (message.Payload.Parts == null)
                    {
                        continue;
                    }

                    int? size = message.Payload.Parts.First()?.Body?.Size;
                    if (size.HasValue && size.Value != 0)
                    {
                        text = message.Payload.Parts.First(p => p.MimeType.ToLowerInvariant().StartsWith("text"))?.Body?.Data;
                    }
                    else
                    {
                        MessagePart part = message.Payload.Parts.First(p => p.MimeType.ToLowerInvariant().StartsWith("multipart"));
                        bool? nested;
                        do
                        {
                            nested = part?.Parts?.Any(p => p.MimeType.ToLowerInvariant().StartsWith("multipart"));
                            if (nested.HasValue && nested.Value == true)
                            {
                                part = part?.Parts?.First(p => p.MimeType.ToLowerInvariant().StartsWith("multipart"));
                            }
                        }
                        while (nested.HasValue && nested.Value == true);

                        text = part?.Parts?.First(p => p.MimeType.ToLowerInvariant().StartsWith("text"))?.Body?.Data;
                    }

                    if (string.IsNullOrEmpty(text) == false)
                    {
                        text = text.Replace("_", "/").Replace("-", "+");
                        text = Encoding.UTF8.GetString(Convert.FromBase64String(text));
                    }

                    woFromEMail.Description = text;

                    requestedWorkOrders.Add(woFromEMail);
                }

                foreach (var email in requestedWorkOrders)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("Requester name: {0}", email.RequesterName));
                    sb.AppendLine(string.Format("Requester email: {0}", email.RequesterEmail));
                    sb.AppendLine(string.Format("Date: {0}", email.ReceivedDate));
                    sb.AppendLine(string.Format("Subject: {0}", email.Subject));
                    sb.AppendLine(string.Format("Description: {0}", email.Description));

                    _workOrdersRepository.Add(new WorkOrder
                    {
                        CompanyId = 1,
                        CreatedDate = DateTime.UtcNow,
                        StatusId = 0,
                        Description = sb.ToString()
                    });
                }

                _workOrdersRepository.SaveChanges();

                return requestedWorkOrders.Count();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                throw new Exception(string.Format("Exception message: {0}{1}Inner exception message: {2}",
                                                  ex.Message,
                                                  Environment.NewLine,
                                                  ex.InnerException?.Message));
            }
        }

        protected bool IsUnread(Message message, string labelId)
        {
            if (message.Id != message.ThreadId)
                return false;

            if (message.LabelIds != null)
                return message.LabelIds.Contains(labelId) == false;

            return true;
        }
    }
}
