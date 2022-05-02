import { ContractExpenseBaseModel } from './contract-expense-base.model';

export class ContractExpenseGridModel extends ContractExpenseBaseModel {

  expenseTypeId: number;
  dailyRate: number;
  dailyTaxRate: number;
  monthlyRate: number;
  yearlyRate: number;

  dailyRateFormula: string;
  monthlyRateFormula: string;
  yearlyRateFormula: string;

  taxesAndInsurance: number;

  constructor(contractExpenseGrid: ContractExpenseGridModel) {
    super(contractExpenseGrid);

    this.dailyRate = contractExpenseGrid.dailyRate || 0;
    this.monthlyRate = contractExpenseGrid.monthlyRate || 0;
    this.yearlyRate = contractExpenseGrid.yearlyRate || 0;
    this.dailyTaxRate = contractExpenseGrid.dailyTaxRate || 0;

    this.taxesAndInsurance = contractExpenseGrid.taxesAndInsurance || 0;
  }
}
