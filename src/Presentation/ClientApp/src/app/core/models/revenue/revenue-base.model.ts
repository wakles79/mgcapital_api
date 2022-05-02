import { CompanyEntity } from '@core/models/common/company-entity.model';

export class RevenueBaseModel extends CompanyEntity {

  buildingId: number;
  customerId: number;
  contractId: number;
  date: Date;
  subTotal: number;
  tax: number;
  total: number;
  description: string;
  reference: string;
  transactionNumber: string;

  constructor(revenueBaseModel: RevenueBaseModel) {
    super(revenueBaseModel);

    this.buildingId = revenueBaseModel.buildingId || 0;
    this.customerId = revenueBaseModel.customerId || 0;
    this.contractId = revenueBaseModel.contractId || 0;
    this.date = revenueBaseModel.date || null;
    this.subTotal = revenueBaseModel.subTotal || 0;
    this.tax = revenueBaseModel.tax || 0;
    this.total = revenueBaseModel.total || 0;
    this.description = revenueBaseModel.description || '';
    this.reference = revenueBaseModel.reference || '';
    this.transactionNumber = revenueBaseModel.transactionNumber || '';
  }
}
