import { FdAttachmentBaseModel } from './fd-attachment-base.model';

export class FdTicketReplyModel {
  public ticket_id: number;
  public attachments: FdAttachmentBaseModel[];
  public body: string;
  public from_email: string;
  public user_id: number;
  public cc_emails: string[];
  public bcc_emails: string[];
  public messageId: string;

  constructor(ticket) {
    this.ticket_id = ticket.ticket_id || 0;
    this.attachments = ticket.attachments || [];
    this.body = ticket.body || '';
    this.from_email = ticket.from_email || null;
    this.user_id = ticket.user_id || 0;
    this.cc_emails = ticket.cc_emails || [];
    this.bcc_emails = ticket.bcc_emails || [];
    this.messageId = ticket.messageId || null;
  }
}
