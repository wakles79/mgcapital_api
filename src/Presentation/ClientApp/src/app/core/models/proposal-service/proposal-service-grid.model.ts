import { ProposalServiceBaseModel } from './proposal-service-base.model';

export class ProposalServiceGridModel extends ProposalServiceBaseModel {

  officeServiceTypeName: string;

  constructor(proposalServiceGrid: ProposalServiceGridModel) {
    super(proposalServiceGrid);

    this.officeServiceTypeName = proposalServiceGrid.officeServiceTypeName || '';
  }

}
