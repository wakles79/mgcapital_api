import { EntityModel } from '../common/entity.model';

export class ProposalSendEmailModel extends EntityModel {

  additionalRecipients: any[];

  constructor(proposalSendEmail: ProposalSendEmailModel) {
    super(proposalSendEmail);
    this.additionalRecipients = proposalSendEmail.additionalRecipients || [];
  }
}
