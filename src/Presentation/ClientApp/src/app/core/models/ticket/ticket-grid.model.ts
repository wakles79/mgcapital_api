import { TicketBaseModel } from '@app/core/models/ticket/ticket-base.model';

export class TicketGridModel extends TicketBaseModel {
  tags: any[];

  constructor(ticket: TicketGridModel) {
    super(ticket);

    this.tags = ticket.tags || [];
  }
}
