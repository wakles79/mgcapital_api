import { EntityModel } from '../common/entity.model';

export class ExpenseSubcategoryBaseModel extends EntityModel {
  name: string;
  expenseTypeId: number;
  rate: number;
  rateType: number;
  periodicity: string;
  status: boolean;


  constructor(expenseSubcategory: ExpenseSubcategoryBaseModel) {
    super(expenseSubcategory);

    this.id = expenseSubcategory.id || -1;
    this.name = expenseSubcategory.name || '';
    this.expenseTypeId = expenseSubcategory.expenseTypeId || -1;
    this.rate = expenseSubcategory.rate || 0;
    this.rateType = expenseSubcategory.rateType || 0;
    this.periodicity = expenseSubcategory.periodicity || '';
    this.status = expenseSubcategory.status || false;
  }
}
