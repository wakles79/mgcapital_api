import { WorkOrderCreateModel } from '@app/core/models/work-order/work-order-create.model';

export class WorkOrderCreateFromTicket extends WorkOrderCreateModel {

  ticketId: number;

  constructor(createModel: any) {
    super(createModel);

    this.ticketId = createModel.ticketId || 0;
  }
}
