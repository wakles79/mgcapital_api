import { EntityModel } from '@core/models/common/entity.model';

export class InspectionItemTicketModel extends EntityModel {

  inspectionItemId: number;
  ticketId: number;
  destinationType: number;
  entityId: number;

  constructor(inspectionItemTicket: InspectionItemTicketModel) {
    super(inspectionItemTicket);

    this.inspectionItemId = inspectionItemTicket.inspectionItemId || 0;
    this.ticketId = inspectionItemTicket.ticketId || 0;
    this.destinationType = inspectionItemTicket.destinationType || null;
    this.entityId = inspectionItemTicket.entityId || null;
  }
}
