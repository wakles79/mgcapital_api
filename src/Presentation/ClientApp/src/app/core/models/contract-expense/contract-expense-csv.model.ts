import { ContractExpenseDefaultType } from './contract-expense-base.model';

export class ContractExpenseCSVModel {
    ContractNumber: string;
    Quantity: number;
    Description: string;
    ExpenseType: string;
    ExpenseSubcategory: number;
    Rate: number;
    RateType: number;
    RatePeriodicity: string;
    Value: number;
    TaxPercent: number;
    DefaultType: ContractExpenseDefaultType;
}
