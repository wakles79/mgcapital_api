import { EntityModel } from '@app/core/models/common/entity.model'
import { FdTicketBaseModel } from './fd-ticket-base.model';
import { FdConversationBaseModel } from './fd-conversation-base.model';
import { timingSafeEqual } from 'crypto';

export class FdTicketDetailModel {
  ticket: FdTicketBaseModel;
  conversations: FdConversationBaseModel[];

  constructor(detail) {
    this.ticket = detail.ticket || null;
    this.conversations = detail.conversations || [];
  }
}
