import { EntityModel } from '../common/entity.model';

export class ContractItemBaseModel extends EntityModel {
  quantity: number;
  description: string;
  contractId: number;
  officeServiceTypeId: number;
  officeServiceTypeName: string;
  rate: number;
  rateType: number;
  ratePeriodicity: string;
  hours: number;
  amount: number;
  rooms: number;
  squareFeet: number;
  defaultType: ContractItemDefaultType;

  constructor(contractItem: ContractItemBaseModel) {
    super(contractItem);

    this.id = contractItem.id || -1;
    this.quantity = contractItem.quantity || 0;
    this.description = contractItem.description || '';
    this.contractId = contractItem.contractId || -1;
    this.officeServiceTypeId = contractItem.officeServiceTypeId || 0;
    this.officeServiceTypeName = contractItem.officeServiceTypeName || '';
    this.rate = contractItem.rate || 0;
    this.rateType = contractItem.rateType || 0;
    this.ratePeriodicity = contractItem.ratePeriodicity || '';
    this.squareFeet = contractItem.squareFeet || 0;
    this.amount = contractItem.amount || 0;
    this.hours = contractItem.hours || 0;
    this.rooms = contractItem.rooms || 0;
    this.defaultType = contractItem.defaultType || ContractItemDefaultType.None;
  }
}

export enum ContractItemDefaultType {
  'None' = 0,
  'OfficeSpace' = 1
}
