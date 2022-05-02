import { ExpenseTypeBaseModel } from './expense-type-base.model';

export class ExpenseTypeGridModel extends ExpenseTypeBaseModel {
  expenseCategoryName: string;
  subcategories: number;

  constructor(expenseTypeGrid: ExpenseTypeGridModel){
    super(expenseTypeGrid);

    this.subcategories = expenseTypeGrid.subcategories || 0;
    this.expenseCategoryName = expenseTypeGrid.expenseCategoryName || '';
  }
}
