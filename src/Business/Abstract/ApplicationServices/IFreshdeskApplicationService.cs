using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Freshdesk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IFreshdeskApplicationService
    {
        Task<FreshdeskTicketBaseViewModel> AddFromTicket(Ticket ticket);

        Task<ConversationBaseViewModel> ReplyTicket(FreshdeskTicketReplyAttachedFilesViewModel obj);

        /// <summary>
        /// Get agent's email signature or the company's default agent
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns></returns>
        Task<string> GetAgentEmailSignature(string id = "", string email = "");

        /// <summary>
        /// Update Freshdesk Ticket Status
        /// </summary>
        /// <param name="ticketId">Freshdesk Ticket Id</param>
        /// <param name="ticketStatus">MgCap Ticket Status</param>
        /// <returns></returns>
        Task UpdateTicketStatus(int ticketId, TicketStatus ticketStatus);

        /// <summary>
        /// Update Freshdesk Ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task UpdateTicket(Ticket ticket);

        Task<bool> DeleteTicket(string id);

        Task<TicketFreshdeskSummaryViewModel> GetTicketDetail(int id);

        byte[] DownloadImageByte(string url);

        IEnumerable<ConversationBaseViewModel> ReadAllTicketConversations(int ticketId);

        bool VerifyAccess(VerifyFresdeshAccessViewModel vm);

        //Task<FreshdeskTicketBaseViewModel> CreateFromTicket(Ticket ticket);

        //Task<TicketFreshdeskSummaryViewModel> GetTicketDetail(int id);

        //Task<IEnumerable<FreshdeskTicketBaseViewModel>> ReadAllTickets();

        //Task<FreshdeskTicketBaseViewModel> GetTicket(int id);

        //Task<IEnumerable<ConversationBaseViewModel>> ReadAllTicketConversations(int ticketId);

        //Task<ConversationBaseViewModel> ReplyTicket(FreshdeskTicketReplyAttachedFilesViewModel obj);

        //Task UpdateTicketStatus(int ticketId, TicketStatus ticketStatus);

        //byte[] DownloadImageByte(string url);

        ///// <summary>
        ///// Get agent's email signature or the company's default agent
        ///// </summary>
        ///// <param name="email">User email</param>
        ///// <returns></returns>
        //Task<string> GetAgentEmailSignature(string id = "", string email = "");
    }
}
