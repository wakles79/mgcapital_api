import { ExpenseBaseModel } from '@core/models/expense/expense-base.model';
export class ExpenseGridModel extends ExpenseBaseModel {

  typeName: string;
  buildingName: string;
  customerName: string;
  epochDate: number;

  constructor(expenseGridModel: ExpenseGridModel) {
    super(expenseGridModel);

    this.typeName = expenseGridModel.typeName || '';
    this.buildingName = expenseGridModel.buildingName || '';
    this.customerName = expenseGridModel.customerName || '';
    this.epochDate = expenseGridModel.epochDate || 0;
  }
}
