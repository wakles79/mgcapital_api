import { CompanyEntity } from '../common/company-entity.model';

export class ProposalBaseModel extends CompanyEntity {

  customerId: number;
  customerName: string;
  customerEmail: string;
  contactId: number;
  location: string;
  status: number;
  statusChangedDate: number;
  billTo: number;
  billToName: string;
  billToEmail: string;


  constructor(proposal: ProposalBaseModel) {
    super(proposal);

    this.customerId = proposal.customerId || 0;
    this.customerName = proposal.customerName || '';
    this.customerEmail = proposal.customerEmail || '';
    this.contactId = proposal.contactId || 0;
    this.location = proposal.location || '';
    this.status = proposal.status || 0;
    this.statusChangedDate = proposal.statusChangedDate || null;
    this.billTo = proposal.billTo || -1;
    this.billToName = proposal.billToName || '';
    this.billToEmail = proposal.billToEmail || '';

  }

}
