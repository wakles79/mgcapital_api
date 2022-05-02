import { EntityModel } from '../common/entity.model';
import { FdAttachmentBaseModel } from '@app/core/models/freshdesk/fd-attachment-base.model';
import { FreshdeskConversationSource } from '@app/core/models/freshdesk/freshdesk-conversation-source-enum';

export class FdConversationBaseModel extends EntityModel {
  public attachments: FdAttachmentBaseModel[];
  public body: string;
  public body_text: string;
  public body_html: string;
  public body_blockquote: string;
  public incoming: boolean;
  public to_emails: string[];
  public private: boolean;
  public source: FreshdeskConversationSource;
  public support_email: string;
  public ticket_id: number;
  public user_id: number;
  public created_at: Date;
  public updated_at: Date;
  public fromActivityLog: boolean;
  public activityType: number;
  public activityTypeName: string;
  public cc_emails: string[];
  public bcc_emails: string[];
  public appCustomFields: { [key: string]: string };

  constructor(conversation) {
    super(conversation);
    this.attachments = conversation.attachments || null;
    this.body = conversation.body || null;
    this.body_text = conversation.body_text || null;
    this.body_html = conversation.body_html || null;
    this.body_blockquote = conversation.body_blockquote || null;
    this.incoming = conversation.incoming || null;
    this.to_emails = conversation.to_emails || [];
    this.private = conversation.private || null;
    this.source = conversation.source || null;
    this.support_email = conversation.support_email || null;
    this.ticket_id = conversation.ticket_id || null;
    this.user_id = conversation.user_id || null;
    this.created_at = conversation.created_at || null;
    this.updated_at = conversation.updated_at || null;

    this.fromActivityLog = conversation.fromActivityLog || false;
    this.activityType = conversation.activityType || 3;
    this.activityTypeName = conversation.activityTypeName || '';
    this.cc_emails = conversation.cc_emails || [];
    this.bcc_emails = conversation.bcc_emails || [];

    this.appCustomFields = conversation.appCustomFields || {};

    if (this.fromActivityLog) {
      this.setUrlLink();
    }
  }

  public setUrlLink(): void {
    let url = window.location.protocol + '//' + window.location.host + '/';
    const entityType = this.appCustomFields['EntityType'];
    if (entityType === '1') {
      if (this.appCustomFields['EntityId']) {
        url += `app/inbox/ticket-detail/${this.appCustomFields['EntityId']}`;
      } else {
        url += 'app/inbox';
      }
    } else if (entityType === '2') {

      if (this.appCustomFields['EntityId']) {
        url += `app/work-orders/detail?action=edit&workorder=${this.appCustomFields['EntityId']}`;
      } else {
        url += 'app/work-orders';
      }
    } else if (entityType === '8') {
      if (this.appCustomFields['EntityId']) {
        url += `app/reports/cleaning-report/${this.appCustomFields['EntityId']}`;
      } else {
        url += 'app/reports/cleaning-report';
      }
    }
    else {
      url += 'app/inbox';
    }
    this.body = this.body.replace(' ((link)) ', url);
    this.body_html = this.body_html.replace(' ((link)) ', url);
  }
}

export enum TicketActivityType {
  FieldUpdated = 0,
  TicketMerged = 1,
  EmailReply = 2,
  None = 3,
  Forwarded = 4,
  AssignedEmployee = 8,
  TicketConverted = 16,
  TicketConvertedWorkOrderSequence = 32
}
