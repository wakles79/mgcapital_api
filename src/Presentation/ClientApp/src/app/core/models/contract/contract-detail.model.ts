import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';

export class ContractDetailModel extends ContractBaseModel {
  customerName: string;
  buildingName: string;

  statusName: string;
  epochExpirationDate: number;

  constructor(contractDetail: ContractDetailModel) {
    super(contractDetail);

    this.customerName = contractDetail.customerName || '';
    this.buildingName = contractDetail.buildingName || '';
    this.statusName = contractDetail.statusName || '';
    this.epochExpirationDate = contractDetail.epochExpirationDate || 0;
  }
}
