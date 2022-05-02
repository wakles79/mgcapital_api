import { TicketDestinationType } from '@app/core/models/ticket/ticket-base.model';

export class ConvertTicketParameters {
  ticketId: number;
  destinationEntityId: number;
  destinationType: TicketDestinationType;

  constructor(ticketId: number = null, destinationEntityId: number = null, destinationType: number = null) {
    this.ticketId = ticketId;
    this.destinationEntityId = destinationEntityId;
    this.destinationType = destinationType;
  }
}
