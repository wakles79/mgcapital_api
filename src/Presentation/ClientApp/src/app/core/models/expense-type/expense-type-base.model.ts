import { CompanyEntity } from '../common/company-entity.model';

export class ExpenseTypeBaseModel extends CompanyEntity {
  expenseCategory: number;
  description: string;
  status: boolean;

  constructor(expenseType: ExpenseTypeBaseModel){
    super(expenseType);

    this.expenseCategory = expenseType.expenseCategory || 0;
    this.description = expenseType.description || '';
    this.status = expenseType.status || false;
  }
}
