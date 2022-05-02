import { EntityModel } from '@app/core/models/common/entity.model';

export class TicketMergeGridModel extends EntityModel {

  number: number;
  description: string;
  status: number;
  statusName: String;
  checked: boolean;

  constructor(ticketMerge) {
    super(ticketMerge);

    this.number = ticketMerge.number || 0;
    this.description = ticketMerge.description || '';
    this.status = ticketMerge.status || '';
    this.statusName = ticketMerge.statusName || '';

  }
}
