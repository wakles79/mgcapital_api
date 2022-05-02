import { ContractBaseModel } from './contract-base.model';
import { ContractItemGridModel } from '../contract-item/contract-item-grid.model';
import { ContractExpenseGridModel } from '../contract-expense/contract-expense-grid.model';
import { RevenueGridModel } from '../revenue/revenue-grid.model';
import { ExpenseGridModel } from '../expense/expense-grid.model';

export class ContractReportDetailsModel extends ContractBaseModel {

  customerName: string;
  buildingName: string;

  contractItems: ContractItemGridModel[];
  contractExpenses: ContractExpenseGridModel[];

  revenues: RevenueGridModel[];
  expenses: ExpenseGridModel[];

  constructor(contractDetail: ContractReportDetailsModel) {
    super(contractDetail);

    this.customerName = contractDetail.customerName || '';
    this.buildingName = contractDetail.buildingName || '';

    this.contractItems = contractDetail.contractItems || [];
    this.contractExpenses = contractDetail.contractExpenses || [];
    this.revenues = contractDetail.revenues || [];
    this.expenses = contractDetail.expenses || [];
  }
}
