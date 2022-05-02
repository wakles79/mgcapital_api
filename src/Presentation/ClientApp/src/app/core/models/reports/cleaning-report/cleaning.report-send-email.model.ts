import { EntityModel } from '../../common/entity.model';

export class CleaningReportSendEmailModel extends EntityModel {

  additionalRecipients: any[];

  constructor(cleaningReportSendEmail: CleaningReportSendEmailModel) {
    super(cleaningReportSendEmail);
    this.additionalRecipients = cleaningReportSendEmail.additionalRecipients || [];
  }
}
