using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.ViewModels.Freshdesk;
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

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class FreshdeskApplicationService : IFreshdeskApplicationService
    {
        /// <summary>
        /// Gets <value>IHttpContextAccessor field</value>
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; }
        private readonly FreshdeskOptions freshdeskOptions;

        public string UserEmail { get; }
        public int CompanyId { get; }

        private readonly ICompanySettingsRepository _CompanySettingsRepository;
        private readonly IEmployeesRepository _EmployeesRepository;

        private RestClient _RestClient { get; set; }
        private string ApiKey { get; set; }
        private string AgentId { get; set; }

        public FreshdeskApplicationService(
            IOptions<FreshdeskOptions> options,
            IHttpContextAccessor httpContextAccessor,
            ICompanySettingsRepository companySettingsRepository,
            IEmployeesRepository employeesRepository)
        {
            this.freshdeskOptions = options.Value;

            this.HttpContextAccessor = httpContextAccessor;
            string StrCompanyId = this.HttpContextAccessor?.HttpContext?.Request?.Headers["CompanyId"];
            this.CompanyId = string.IsNullOrEmpty(StrCompanyId) ? 1 : int.Parse(StrCompanyId);
            this.UserEmail = this.HttpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";

            this._CompanySettingsRepository = companySettingsRepository;
            this._EmployeesRepository = employeesRepository;
        }

        private async Task LoadCredentials()
        {
            Employee employee = await this._EmployeesRepository.GetByEmailAndCompanyAsync(this.UserEmail, this.CompanyId);
            if (employee == null)
            {
                CompanySettings companySettings = await this._CompanySettingsRepository.SingleOrDefaultAsync(c => c.CompanyId == this.CompanyId);
                this.ApiKey = companySettings.FreshdeskDefaultApiKey;
                this.AgentId = companySettings.FreshdeskDefaultAgentId;
            }
            else
            {
                if (employee.HasFreshdeskAccount)
                {
                    this.ApiKey = employee.FreshdeskApiKey;
                    this.AgentId = employee.FreshdeskAgentId;
                }
                else
                {
                    CompanySettings companySettings = await this._CompanySettingsRepository.SingleOrDefaultAsync(c => c.CompanyId == this.CompanyId);
                    this.ApiKey = companySettings.FreshdeskDefaultApiKey;
                    this.AgentId = companySettings.FreshdeskDefaultAgentId;
                }
            }
        }

        public async Task<FreshdeskTicketBaseViewModel> AddFromTicket(Ticket ticket)
        {
            try
            {
                #region FreshDesk
                await this.LoadCredentials();

                FreshdeskTicketCreateViewModel create = new FreshdeskTicketCreateViewModel()
                {
                    Subject = ticket.Description.Length > 25 ? $"{ticket.Description.Substring(0, 25)}..." : ticket.Description,
                    Name = ticket.RequesterFullName,
                    Email = ticket.RequesterEmail,
                    Description = ticket.Description,
                    Status = FreshdeskTicketStatus.Pending,
                    Priority = FreshdeskTicketPriority.Low,
                    Tags = new List<string>() { "from_mgcap_website" }
                };

                RestRequest request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    subject = create.Subject,
                    name = create.Name,
                    email = create.Email,
                    description = create.Description,
                    status = create.Status,
                    priority = create.Priority,
                    tags = create.Tags
                });

                var result = this.MakeHttpRequest<FreshdeskTicketBaseViewModel>("tickets", request);

                return result;
                #endregion FreshDesk
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ConversationBaseViewModel> ReplyTicket(FreshdeskTicketReplyAttachedFilesViewModel obj)
        {
            TicketReplyViewModel reply = JsonConvert.DeserializeObject<TicketReplyViewModel>(obj.ReplyModel);
            await this.LoadCredentials();

            reply.UserId = long.Parse(this.AgentId);

            string signature = await this.GetAgentEmailSignature(id: this.AgentId);

            if (!string.IsNullOrEmpty(signature))
            {
                int divSpaces = 5;
                int index = reply.Body.IndexOf("class=\"quoted-text\"");
                index -= divSpaces;
                if (index >= 0)
                {
                    reply.Body = reply.Body.Insert(index, $"<br>{signature}<br>");
                }
                else
                {
                    reply.Body += $"<br>{signature}";
                }
            }

            string contentType = "application/json";
            RestRequest request = new RestRequest(Method.POST);

            if (obj.Attachments != null)
            {
                string authInfo = $"{this.ApiKey}:X";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                contentType = "multipart/form-data";
                request.AddParameter("body", reply.Body);
                request.AddParameter("user_id", reply.UserId);

                foreach (string email in reply.CcEmails)
                {
                    request.AddParameter("cc_emails[]", email);
                }

                foreach (string email in reply.BccEmails)
                {

                    request.AddParameter("bcc_emails[]", email);
                }

                foreach (var attachment in obj.Attachments)
                {
                    MemoryStream ms = new MemoryStream();
                    attachment.CopyTo(ms);
                    request.AddFileBytes("attachments[]", ms.ToArray(), attachment.FileName);
                    ms.Close();
                }
            }
            else
            {
                contentType = "application/json";
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    body = reply.Body,
                    from_email = reply.FromEmail,
                    user_id = reply.UserId,
                    cc_emails = reply.CcEmails,
                    bcc_emails = reply.BccEmails
                });
            }

            ConversationBaseViewModel result = this.MakeHttpRequest<ConversationBaseViewModel>($"tickets/{reply.TicketId}/reply", request, contentType);
            return result;
        }

        public async Task<string> GetAgentEmailSignature(string id = "", string email = "")
        {
            try
            {
                if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(email))
                {
                    await this.LoadCredentials();
                    id = this.AgentId;
                }

                string signature = string.Empty;

                IEnumerable<FreshdeskAgentBaseViewModel> agents = new List<FreshdeskAgentBaseViewModel>();
                if (!string.IsNullOrEmpty(id))
                {
                    var result = this.GetAsync<FreshdeskAgentBaseViewModel>($"agents/{id}");
                    if (result != null)
                    {
                        agents = new List<FreshdeskAgentBaseViewModel>() { result };
                    }
                }
                else
                {
                    agents = this.GetAsync<IEnumerable<FreshdeskAgentBaseViewModel>>($"agents?email={email}");
                }

                if (agents.Count() > 0)
                {
                    signature = agents.FirstOrDefault().Signature;
                }
                else
                {
                    var companySettings = await this._CompanySettingsRepository.SingleOrDefaultAsync(c => c.CompanyId == this.CompanyId);
                    if (companySettings != null)
                    {
                        var agent = this.GetAsync<FreshdeskAgentBaseViewModel>($"agents/{companySettings.FreshdeskDefaultAgentId}");
                        if (agent != null)
                        {
                            signature = agent.Signature;
                        }
                    }
                }
                return signature;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task UpdateTicketStatus(int ticketId, TicketStatus ticketStatus)
        {
            try
            {
                await this.LoadCredentials();

                FreshdeskTicketStatus freshdeskTicketStatus = FreshdeskTicketStatus.Open;
                if (ticketStatus == TicketStatus.Resolved || ticketStatus == TicketStatus.Converted)
                {
                    freshdeskTicketStatus = FreshdeskTicketStatus.Closed;
                }
                else if (ticketStatus == TicketStatus.Undefined || ticketStatus == TicketStatus.Draft)
                {
                    freshdeskTicketStatus = FreshdeskTicketStatus.Open;
                }

                RestRequest request = new RestRequest(Method.PUT);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new { status = (int)freshdeskTicketStatus });

                this.MakeHttpRequest<FreshdeskTicketBaseViewModel>($"tickets/{ticketId}", request);
            }
            catch (Exception ex)
            {
                // TODO
            }
        }

        /// <summary>
        /// Update Freshdesk Ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public async Task UpdateTicket(Ticket ticket)
        {
            try
            {
                if (!ticket.FreshdeskTicketId.HasValue)
                {
                    return;
                }

                await this.LoadCredentials();

                FreshdeskTicketStatus freshdeskTicketStatus = FreshdeskTicketStatus.Pending;
                if (ticket.Status == TicketStatus.Resolved || ticket.Status == TicketStatus.Converted)
                {
                    freshdeskTicketStatus = FreshdeskTicketStatus.Closed;
                }
                else if (ticket.Status == TicketStatus.Undefined)
                {
                    freshdeskTicketStatus = FreshdeskTicketStatus.Open;
                }

                RestRequest request = new RestRequest(Method.PUT);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    status = (int)freshdeskTicketStatus
                    //description = ticket.Description
                });

                this.MakeHttpRequest<FreshdeskTicketBaseViewModel>($"tickets/{ticket.FreshdeskTicketId.Value}", request);
            }
            catch (Exception ex)
            {
                // TODO
            }
        }

        public async Task<bool> DeleteTicket(string id)
        {
            await this.LoadCredentials();

            return this.DeleteAsync($"tickets/{id}");
        }

        public async Task<TicketFreshdeskSummaryViewModel> GetTicketDetail(int id)
        {
            var result = new TicketFreshdeskSummaryViewModel();
            await this.LoadCredentials();

            var ticket = this.GetTicket(id);
            if (ticket != null)
            {
                result.Ticket = ticket;
                if (ticket.RequesterId.HasValue)
                {
                    string email = string.Empty;
                    var contact = this.GetContact(ticket.RequesterId.Value);
                    if (contact == null)
                    {
                        var agent = this.GetAgent(ticket.RequesterId.Value);
                        email = agent == null ? "" : agent.Contact.Email;
                    }
                    else
                    {
                        email = contact == null ? "" : contact.Email;
                    }

                    result.Ticket.Email = email;
                }
            }

            var conversations = this.ReadAllTicketConversations(id);

            result.Conversations = conversations.Count() == 0 ? new HashSet<ConversationBaseViewModel>() : conversations;

            return result;
        }

        public FreshdeskTicketBaseViewModel GetTicket(int id)
        {
            try
            {
                var result = this.GetAsync<FreshdeskTicketBaseViewModel>($"tickets/{id}");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FreshdeskAgentBaseViewModel GetAgent(long id)
        {
            try
            {
                var result = this.GetAsync<FreshdeskAgentBaseViewModel>($"agents/{id}");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FreshdeskContactBaseViewModel GetContact(long id)
        {
            try
            {

                var result = this.GetAsync<FreshdeskContactBaseViewModel>($"contacts/{id}");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<ConversationBaseViewModel> ReadAllTicketConversations(int ticketId)
        {
            try
            {
                var result = this.GetAsync<IEnumerable<ConversationBaseViewModel>>($"tickets/{ticketId}/conversations");
                return result;
            }
            catch (Exception ex)
            {
                return new HashSet<ConversationBaseViewModel>();
            }
        }
        public byte[] DownloadImageByte(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                var aBytes = webClient.DownloadData(url);
                return aBytes;
            }
        }

        public bool VerifyAccess(VerifyFresdeshAccessViewModel vm)
        {
            try
            {
                this.ApiKey = vm.Key;
                long id = long.Parse(vm.AgentId);
                var result = this.GetAsync<FreshdeskAgentBaseViewModel>($"agents/{id}");
                return result == null ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Request
        /// <summary>
        /// Make Get HTTP Request
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="action">EndPoint to be called</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public T GetAsync<T>(string action, IDictionary<string, string> parameters = null, string contentType = "application/json")
        {
            try
            {
                this._RestClient = new RestClient(string.Format(this.freshdeskOptions.BaseUrl, this.freshdeskOptions.FreshdeskDomain));
                var request = new RestRequest(action, Method.GET);

                string authInfo = $"{this.ApiKey}:X";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                request.AddHeader("Content-Type", contentType);
                request.AddHeader("Authorization", $"Basic {authInfo}");

                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        request.AddParameter(item.Key, item.Value);
                    }
                }

                var response = this._RestClient.Get(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<T>(response.Content);
                    return result;
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public T MakeHttpRequest<T>(string action, RestRequest request, string contentType = "application/json")
        {
            try
            {
                this._RestClient = new RestClient(string.Format(this.freshdeskOptions.BaseUrl, this.freshdeskOptions.FreshdeskDomain) + action);

                string authInfo = $"{this.ApiKey}:X";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                request.AddHeader("Content-Type", contentType);
                request.AddHeader("Authorization", $"Basic {authInfo}");

                IRestResponse response = _RestClient.Execute(request);
                if (response.IsSuccessful)
                {
                    string contentResponse = response.Content;
                    var result = JsonConvert.DeserializeObject<T>(contentResponse);
                    return result;
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool DeleteAsync(string action)
        {
            try
            {

                this._RestClient = new RestClient(string.Format(this.freshdeskOptions.BaseUrl, this.freshdeskOptions.FreshdeskDomain) + action);

                string authInfo = $"{this.ApiKey}:X";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                RestRequest request = new RestRequest(Method.DELETE);
                request.AddHeader("Authorization", $"Basic {authInfo}");

                IRestResponse response = _RestClient.Execute(request);

                if (response.IsSuccessful)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public T PostAsync<T>(string action, RestRequest request, string contentType = "application/json")
        //{
        //    try
        //    {
        //        this._RestClient = new RestClient(string.Format(this.freshdeskOptions.BaseUrl, this.freshdeskOptions.FreshdeskDomain) + action);

        //        string authInfo = $"{this.ApiKey}:X";
        //        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

        //        request.AddHeader("Content-Type", contentType);
        //        request.AddHeader("Authorization", $"Basic {authInfo}");

        //        IRestResponse response = _RestClient.Execute(request);
        //        if (response.IsSuccessful)
        //        {
        //            string contentResponse = response.Content;
        //            var result = JsonConvert.DeserializeObject<T>(contentResponse);
        //            return result;
        //        }
        //        else
        //        {
        //            throw new Exception(response.StatusCode.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception();
        //    }
        //}

        //public T PutAsync<T>(string action, RestRequest request, string contentType = "application/json")
        //{
        //    try
        //    {
        //        this._RestClient = new RestClient(string.Format(this.freshdeskOptions.BaseUrl, this.freshdeskOptions.FreshdeskDomain) + action);

        //        string authInfo = $"{this.ApiKey}:X";
        //        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

        //        request.AddHeader("Content-Type", contentType);
        //        request.AddHeader("Authorization", $"Basic {authInfo}");

        //        IRestResponse response = _RestClient.Execute(request);
        //        if (response.IsSuccessful)
        //        {
        //            string contentResponse = response.Content;

        //            var result = JsonConvert.DeserializeObject<T>(contentResponse);
        //            return result;
        //        }
        //        else
        //        {
        //            throw new Exception(response.StatusCode.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception();
        //    }
        //}

        private string ConvertStringArguments(IDictionary<string, string> arguments)
        {
            string urlArguments = string.Empty;
            foreach (var arg in arguments)
            {
                if (urlArguments == string.Empty)
                    urlArguments = arg.Value == string.Empty ? string.Empty : $"?{arg.Key}={arg.Value}";
                else
                    urlArguments += arg.Value == string.Empty ? string.Empty : $"&{arg.Key}={arg.Value}";
            }
            return urlArguments;
        }

        #endregion
    }
}
