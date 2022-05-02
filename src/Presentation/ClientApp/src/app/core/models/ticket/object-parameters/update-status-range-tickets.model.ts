import { TicketStatus } from '@app/core/models/ticket/ticket-base.model';

export class UpdateStatusRangeTicketsParameters {

  status: TicketStatus;
  id: number[];

  constructor(id: number[] = null, status: TicketStatus = null ) {
    this.id = id;
    this.status = status;
  }
}
