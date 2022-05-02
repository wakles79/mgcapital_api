import { ContractBaseModel } from '@core/models/contract/contract-base.model';
import { RevenueGridModel } from '../revenue/revenue-grid.model';
import { ExpenseGridModel } from '../expense/expense-grid.model';

export class ContractTrackingDetailModel extends ContractBaseModel {

  buildingName: string;
  customerName: string;

  revenues: RevenueGridModel[];
  expenses: ExpenseGridModel[];

  constructor(contractTrackingModel: ContractTrackingDetailModel) {
    super(contractTrackingModel);

    this.buildingName = contractTrackingModel.buildingName || '';
    this.customerName = contractTrackingModel.customerName || '';

    this.revenues = contractTrackingModel.revenues || [];
    this.expenses = contractTrackingModel.expenses || [];
  }
}
