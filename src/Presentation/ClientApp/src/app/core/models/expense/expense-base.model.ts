import { CompanyEntity } from '@core/models/common/company-entity.model';

export class ExpenseBaseModel extends CompanyEntity {

  isDirect: boolean;
  contractId: number;
  buildingId: number;
  customerId: number;
  type: ExpenseCategory;
  reference: string;
  amount: number;
  vendor: string;
  description: string;
  date: Date;
  transactionNumber: string;

  constructor(expenseBaseModel: ExpenseBaseModel) {
    super(expenseBaseModel);

    this.isDirect = expenseBaseModel.isDirect || true;
    this.contractId = expenseBaseModel.contractId || null;
    this.buildingId = expenseBaseModel.buildingId || null;
    this.customerId = expenseBaseModel.customerId || null;
    this.type = expenseBaseModel.type || 0;
    this.reference = expenseBaseModel.reference || '';
    this.amount = expenseBaseModel.amount || 0;
    this.vendor = expenseBaseModel.vendor || '';
    this.description = expenseBaseModel.description || '';
    this.date = expenseBaseModel.date || null;
    this.transactionNumber = expenseBaseModel.transactionNumber || '';
  }

}

export enum ExpenseCategory {
  Labor = 0,
  Equipments = 1,
  Supplies = 2,
  Others = 3,
  Subcontractor = 4
}
