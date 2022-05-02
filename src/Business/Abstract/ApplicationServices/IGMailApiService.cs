using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.GMailApi;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MGCap.Domain.ViewModels.Freshdesk;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IGMailApiService
    {
        //string UserEmail { get; }

        GoogleCredential GetCredential();

        GmailService GetService();

        List<GMailEmailAttachment> GetAttachments(string messageId, string outputDir);

        Message SendEmail(string to, string from, string subject, string bodyText, bool IsHtml,IEnumerable<string> cc,IEnumerable<string> bcc);

        Task<Message> ReplyEmail(Message replayMSG, FreshdeskTicketReplyAttachedFilesViewModel body, int ticketId);

        Message GetMessagesById(GmailService gmailService, string userEmail, string MessagesId);

        void GetThread(GmailService service);

        string Base64Decode(string base64Text);

        string Base64ToByte(string base64Text);

        void MarkAsRead(string hostEmailAddress, string messageId);

        Message GetMessageContent(string messageId);

        Task<IEnumerable<ConversationBaseViewModel>> GetConversations(string messageId, int ticketId);

        Task<GMailApiTicketBaseViewModel> GetEmailData(Message msg, int? ticketId = null);

        Task<string> AddFromTicket(Ticket ticket);

        Task<ListHistoryResponse> GetHistory(ulong historyId);

        string ExtractEmailFromText(string text);

        // Runs Watch Gmail API command to start push notifications
        ulong? StartService();

        // Runs Stop Gmail API command to start push notifications
        void StopService();
    }
}
