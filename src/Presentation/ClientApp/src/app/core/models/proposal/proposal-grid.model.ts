import { ProposalBaseModel } from './proposal-base.model';

export class ProposalGridModel extends ProposalBaseModel {

  lineItems: number;
  customerName: string;
  contactName: string;
  statusName: string;
  createdDate: number;
  value: number;

  constructor(proposalGridModel: ProposalGridModel) {
    super(proposalGridModel);

    this.lineItems = proposalGridModel.lineItems || 0;
    this.customerName = proposalGridModel.customerName || '';
    this.contactName = proposalGridModel.contactName || '';
    this.statusName = proposalGridModel.statusName || '';
    this.createdDate = proposalGridModel.createdDate || 0;
    this.value = proposalGridModel.value || 0;

  }

}
