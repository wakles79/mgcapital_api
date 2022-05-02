import { EntityModel } from '../common/entity.model';

export class ProposalServiceBaseModel extends EntityModel {
  proposalId: number;
  buildingId: number;
  buildingName: string;
  officeServiceTypeId: number;
  quantity: number;
  requesterName: string;
  description: string;
  location: string;
  rate: number;
  dateToDelivery: number;

  constructor(proposalService: ProposalServiceBaseModel) {
    super(proposalService);

    this.proposalId = proposalService.proposalId || 0;
    this.buildingId = proposalService.buildingId || 0;
    this.buildingName = proposalService.buildingName || '';
    this.officeServiceTypeId = proposalService.officeServiceTypeId || 0;
    this.quantity = proposalService.quantity || 0;
    this.requesterName = proposalService.requesterName || '';
    this.description = proposalService.description || '';
    this.location = proposalService.location || '';
    this.rate = proposalService.rate || 0;
    this.dateToDelivery = proposalService.dateToDelivery || null;

  }
}
