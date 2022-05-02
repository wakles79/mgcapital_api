import { CompanyEntity } from '@core/models/common/company-entity.model';
import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class TicketAttachmentBaseModel extends AttachmentBaseModel {

  ticketId: number;

  constructor(tickectAttachment: TicketAttachmentBaseModel) {
    super(tickectAttachment);
    this.ticketId = tickectAttachment.ticketId || null;
  }
}
