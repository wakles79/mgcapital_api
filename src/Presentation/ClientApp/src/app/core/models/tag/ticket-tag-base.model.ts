import { EntityModel } from '@app/core/models/common/entity.model';

export class TicketTagBaseModel extends EntityModel {
  ticketId: number;
  tagId: number;
  constructor(ticketTag) {
    super(ticketTag);
    this.ticketId = ticketTag.ticketId || 0;
    this.tagId = ticketTag.tagId || 0;
  }
}
