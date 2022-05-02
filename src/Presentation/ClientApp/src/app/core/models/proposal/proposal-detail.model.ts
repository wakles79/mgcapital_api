import { ProposalBaseModel } from './proposal-base.model';
import { ProposalServiceGridModel } from '../proposal-service/proposal-service-grid.model';

export class ProposalDetailModel extends ProposalBaseModel {

  contactName: string;
  statusName: string;
  proposalServices: ProposalServiceGridModel[];
  createdDate: number;


  constructor(proposal: ProposalDetailModel) {
    super(proposal);

    this.proposalServices = proposal.proposalServices || [];
    this.contactName = proposal.contactName || '';
    this.statusName = proposal.statusName || '';
    this.createdDate = proposal.createdDate || 0;
  }
}
