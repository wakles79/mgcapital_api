
export class TicketAdditionalData {
  location: string;
  cleaningReportId: number;

  constructor(ticketAdditionalData: TicketAdditionalData = null) {
    if (ticketAdditionalData) {
      this.location = ticketAdditionalData.location;
      this.cleaningReportId = ticketAdditionalData.cleaningReportId;
    }
    else {
      this.location = '';
      this.cleaningReportId = null;
    }
  }
}
