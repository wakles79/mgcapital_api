import { EntityModel } from '../common/entity.model';

export class ContractExpenseBaseModel extends EntityModel {
  quantity: number;
  description: string;
  contractId: number;
  expenseTypeId: number;
  expenseTypeName: string;
  expenseCategory: number;
  rate: number;
  rateType: number;
  ratePeriodicity: string;
  expenseSubcategoryId: number;
  expenseSubcategoryName: string;
  value: number;
  overheadPercent: number;
  defaultType: ContractExpenseDefaultType;

  constructor(contractExpense: ContractExpenseBaseModel) {
    super(contractExpense);

    this.id = contractExpense.id || 0;
    this.quantity = contractExpense.quantity || 0;
    this.description = contractExpense.description || '';
    this.contractId = contractExpense.contractId || 0;
    this.expenseTypeId = contractExpense.expenseTypeId || 0;
    this.expenseTypeName = contractExpense.expenseTypeName || '';
    this.expenseCategory = contractExpense.expenseCategory || 0;
    this.rate = contractExpense.rate || 0;
    this.rateType = contractExpense.rateType || 0;
    this.ratePeriodicity = contractExpense.ratePeriodicity || '';
    this.expenseSubcategoryId = contractExpense.expenseSubcategoryId || 0;
    this.expenseSubcategoryName = contractExpense.expenseSubcategoryName || '';
    this.value = contractExpense.value || 0;
    this.overheadPercent = contractExpense.overheadPercent || 0;
    this.defaultType = contractExpense.defaultType || ContractExpenseDefaultType.None;
  }
}

export enum ContractExpenseDefaultType {
  'None' = 0,
  'Worker' = 1,
  'Supervisor' = 2,
  'Dayporter' = 3,
  'AdminOperations' = 4,
  'VanCrew' = 5,
  'Supplies' = 6
}
