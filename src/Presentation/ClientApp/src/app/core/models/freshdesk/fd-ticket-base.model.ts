import { FdAttachmentBaseModel } from '@app/core/models/freshdesk/fd-attachment-base.model';
import { FreshdeskTicketPriority } from '@app/core/models/freshdesk/freshdesk-ticket-priority-enum';
import { FreshdeskTicketSource } from '@app/core/models/freshdesk/freshdesk-ticket-source-enum';
import { FreshdeskTicketStatus } from '@app/core/models/freshdesk/freshdesk-ticket-status-enum';
import { EntityModel } from '../common/entity.model';

export class FdTicketBaseModel extends EntityModel {

  public attachments: FdAttachmentBaseModel[];
  public cc_emails: string[];
  public company_id: number;
  public custom_fields: any;
  public deleted: boolean;
  public description: string;
  public description_text: string;
  public due_by: Date;
  public email: string;
  public email_config_id: number;
  public facebook_id: string;
  public fr_due_by: Date;
  public fr_escalated: boolean;
  public fwd_emails: string[];
  public group_id: number;
  public is_escalated: boolean;
  public name: string;
  public phone: string;
  public priority: FreshdeskTicketPriority;
  public priorityName: string;
  public product_id: number;
  public reply_cc_emails: string[];
  public requester_id: number;
  public responder_id: number;
  public source: FreshdeskTicketSource;
  public sourceName: string;
  public spam: boolean;
  public status: FreshdeskTicketStatus;
  public statusName: string;
  public subject: string;
  public tags: string[];
  public to_emails: string[];
  public twitter_id: string;
  public type: string;
  public created_at: Date;
  public updated_at: Date;
  public messageId: string;

  constructor(ticket) {
    super(ticket);

    this.attachments = ticket.attachments || [];
    this.cc_emails = ticket.cc_emails || [];
    this.company_id = ticket.company_id || null;
    this.custom_fields = ticket.custom_fields || null;
    this.deleted = ticket.deleted || null;
    this.description = ticket.description || null;
    this.description_text = ticket.description_text || null;
    this.due_by = ticket.due_by || null;
    this.email = ticket.email || null;
    this.email_config_id = ticket.email_config_id || null;
    this.facebook_id = ticket.facebook_id || null;
    this.fr_due_by = ticket.fr_due_by || null;
    this.fr_escalated = ticket.fr_escalated || null;
    this.fwd_emails = ticket.fwd_emails || [];
    this.group_id = ticket.group_id || null;
    this.is_escalated = ticket.is_escalated || null;
    this.name = ticket.name || null;
    this.phone = ticket.phone || null;
    this.priority = ticket.priority || null;
    this.priorityName = ticket.priorityName || '';
    this.product_id = ticket.product_id || null;
    this.reply_cc_emails = ticket.reply_cc_emails || [];
    this.requester_id = ticket.requester_id || null;
    this.responder_id = ticket.responder_id || null;
    this.source = ticket.source || null;
    this.sourceName = ticket.sourceName || '';
    this.spam = ticket.spam || null;
    this.status = ticket.status || null;
    this.statusName = ticket.statusName || '';
    this.subject = ticket.subject || null;
    this.tags = ticket.tags || [];
    this.to_emails = ticket.to_emails || [];
    this.twitter_id = ticket.twitter_id || null;
    this.type = ticket.type || null;
    this.created_at = ticket.created_at || null;
    this.updated_at = ticket.updated_at || null;
    this.messageId = ticket.messageId || null;
  }
}
