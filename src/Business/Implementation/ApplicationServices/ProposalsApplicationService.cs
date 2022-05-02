// -----------------------------------------------------------------------
// <copyright file="ProposalsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Proposal;
using MGCap.Domain.ViewModels.ProposalService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ProposalsApplicationService : BaseSessionApplicationService<Proposal, int>, IProposalsApplicationService
    {
        public new IProposalsRepository Repository => base.Repository as IProposalsRepository;
        private readonly IProposalServicesRepository _proposalServicesRepository;
        private readonly IEmployeesRepository EmployeesRepository;
        private readonly IEmailSender EmailSender;
        private readonly IEmailActivityLogRepository EmailActivityLogRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProposalsApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="ProposalsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public ProposalsApplicationService(
            IEmailSender emailSender,
            IProposalsRepository repository,
            IProposalServicesRepository proposalServicesRepository,
            IEmployeesRepository employeesRepository,
            IEmailActivityLogRepository emailActivityLogRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            this._proposalServicesRepository = proposalServicesRepository;
            this.EmployeesRepository = employeesRepository;
            this.EmailSender = emailSender;
            this.EmailActivityLogRepository = emailActivityLogRepository;
        }

        public Task<DataSource<ProposalGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? status = -1)
        {
            return Repository.ReadAllDapperAsync(request, this.CompanyId, status);
        }

        public Task<ProposalReportDetailViewModel> GetProposalReportDetailsDapperAsync(int? proposalId, Guid? guid)
        {
            return this.Repository.GetProposalReportDetailsDapperAsync(proposalId, guid);
        }

        public async Task<Proposal> UpdateStatusAsync(int proposalId, int newStatus, string billToName, string billToEmail, int? billTo = -1)
        {
            var proposal = await this.Repository.SingleOrDefaultAsync(p => p.ID == proposalId);
            proposal.Status = newStatus;
            proposal.StatusChangedDate = DateTime.UtcNow;
            proposal.BillTo = billTo == -1 ? null : billTo;
            proposal.BillToName = billToName;
            proposal.BillToEmail = billToEmail;
            return await this.Repository.UpdateAsync(proposal);
        }

        public async Task SendProposalReport(ProposalReportDetailViewModel vm, IEnumerable<ProposalAdditionalRecipientViewModel> additionalRecipients, bool commentResponse = false)
        {
            string template = commentResponse ? ProposalCommentResponseEmailTemplate : ProposalEmailTemplate;
            var emailBody = this.GetLinkContentFromTemplate(vm, template + Signature);

            try
            {
                string fromDisplay = string.Empty;
                // Gets logged employee's id
                var employeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);

                if (employeeId > 0)
                {
                    var employee = await this.EmployeesRepository.SingleOrDefaultDapperAsync(employeeId, this.CompanyId);
                    if (employee != null)
                    {
                        // Brace yourself, a lot of null-conditional operators are coming
                        fromDisplay = employee
                                        ?.FullName
                                        ?.RemoveDuplicatedSpaces();
                    }
                }

                IEnumerable<string> cc = null;
                List<EmailLogEntry> emailLogEntries = new List<EmailLogEntry>();
                if (additionalRecipients != null && additionalRecipients.Any())
                {
                    cc = additionalRecipients.Select(ar => ar.Email);
                    foreach (var ar in additionalRecipients)
                    {
                        emailLogEntries.Add(new EmailLogEntry()
                        {
                            Email = ar.Email,
                            Name = ar.FullName
                        });
                    }
                }

                await this.EmailSender.SendEmailAsync(
                    vm.CustomerEmail,
                    "Proposal Details",
                    plainTextMessage: emailBody,
                    htmlMessage: emailBody,
                    fromDisplay: fromDisplay,
                    replyTo: this.UserEmail,
                    cc: cc);

                // Register Activity
                await EmailActivityLogRepository.AddAsync(new EmailActivityLog()
                {
                    CompanyId = this.CompanyId,
                    Subject = "Proposal Details",
                    SentTo = vm.CustomerEmail,
                    Body = emailBody,
                    SharedUrl = $"{this.AppBaseUrl}proposals/proposal-detail/{vm.Guid}",
                    Cc = emailLogEntries
                });

                await this.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                await this.EmailSender.SendEmailAsync(
                            "axzesllc@gmail.com",
                            "Error Sending Emails (Proposals)",
                            plainTextMessage: exception.Message);
                throw exception;
                throw;
            }
        }

        private string GetLinkContentFromTemplate(ProposalReportDetailViewModel vm, string template)
        {
            string content = string.Empty;

            if (string.IsNullOrEmpty(template) || vm == null)
                return content;

            content = template.Replace("[To]", vm.CustomerName.ToString())
                                 .Replace("[CreatedDate]", vm.CreatedDate.ToShortDateString())
                                 .Replace("[Link]", $"{this.AppBaseUrl}proposals/proposal-detail/{vm.Guid}")
                                 .Replace("[From]", vm.ContactName);

            return content;
        }

        protected static string ProposalEmailTemplate = @"<p>Dear [To],</p>
            <br/><p>The proposal created at [CreatedDate] is ready to be viewed. <br/>
            <br/>
            <a href='[Link]'>Click here to view the proposal.</a></p><br>
            <p>Thank you, <br/>
            [From]</p>";

        protected static string ProposalCommentResponseEmailTemplate = @"<p>Dear [To],</p><br/>
            <p>MG Capital has responded to your comment.<br/>
            <br/>
            <a href='[Link]'>Click here to view your proposal.</a></p><br>
            <p>Thank you, <br/>
            [From]</p>";

        public static string Signature = @"
            <small>MG Capital Maintenance Inc, Customer Service Department</small>
            <br>
            <small>110 Pheasant Wood Ct Suite D Morrisville, NC. &nbsp;27560</small>
            <br>
            <small>C:</small><small>919 337 2304 &nbsp;</small>
            <small>O:</small><small>&nbsp;919 461 8573 &nbsp;</small>
            <small>F</small><small>: 919 467 0837</small>
            <small>Hours</small><small>:Mon-Fri, 8am-5pm </small>
            <br>
            <small>
            Print this email only if you must. &nbsp;MG Capital &nbsp;is a Member of the US Green Building Council.
            </small>
            <br>
            <img src='http://mgcapitalmain.com/wordpress/wp-content/uploads/2018/07/img1.jpg' alt='Logo 1'>
            <img src='http://mgcapitalmain.com/wordpress/wp-content/uploads/2018/07/img2.jpg' alt='Logo 2'>";

        public Task<ProposalService> AddProposalServiceAsync(ProposalService proposalService)
        {
            return this._proposalServicesRepository.AddAsync(proposalService);
        }

        public Task<ProposalService> UpdateProposalServiceAsync(ProposalService proposalService)
        {
            return this._proposalServicesRepository.UpdateAsync(proposalService);
        }

        public Task<ProposalService> GetProposalServiceByIdAsync(int id)
        {
            return this._proposalServicesRepository.SingleOrDefaultAsync(p => p.ID == id);
        }

        public Task<DataSource<ProposalServiceGridViewModel>> ReadAllProposalServicesDapperAsync(DataSourceRequest request, int proposalId)
        {
            return this._proposalServicesRepository.ReadAllDapperAsync(request, proposalId);
        }
    }
}
