import { EntityModel } from '@core/models/common/entity.model';
import { InspectionAdditionalRecipientModel } from '@core/models/inspections/inspection-additional-recipient.model';

export class InspectionSendEmailModel extends EntityModel {
  additionalRecipients: InspectionAdditionalRecipientModel[];

  constructor(proposalSendEmail: InspectionSendEmailModel) {
    super(proposalSendEmail);
    this.additionalRecipients = proposalSendEmail.additionalRecipients || [];
  }
}
