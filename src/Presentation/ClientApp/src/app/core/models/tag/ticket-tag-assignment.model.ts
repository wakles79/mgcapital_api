export class TicketTagAssignment {
  ticketTagId: number;
  tagId: number;
  description: string;
  hexColor: string;

  constructor(ticketTag) {
    this.ticketTagId = ticketTag.ticketTagId || 0;
    this.tagId = ticketTag.tagId || 0;
    this.description = ticketTag.description || 0;
    this.hexColor = ticketTag.hexColor || '#000000';
  }

}
