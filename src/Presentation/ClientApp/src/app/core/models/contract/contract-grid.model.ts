import { CompanyEntity } from '@core/models/common/company-entity.model';

export class ContractGridModel extends CompanyEntity {
  contractNumber: string;
  customerId: number;
  customerFullName: string;
  customerCode: string;
  buildingName: string;
  buildingCode: string;
  status: number;
  totalItems: number;
  monthlyRate: number;
  expirationDateEpoch: number;
  occupiedSquareFeets: number;
  unoccupiedSquareFeets: number;
  updatedDate: Date;
  monthlyProfit: number;
  MonthlyProfitRatio: number;
  totalMonthlyRevenue: number;
  totalMonthlyExpense: number;
  totalMonthlyLaborExpense: number;

  constructor(contract: ContractGridModel) {
    super(contract);
    this.contractNumber = contract.contractNumber || '';
    this.customerFullName = contract.customerFullName || '';
    this.buildingName = contract.buildingName || '';
    this.status = contract.status || -1;
    this.occupiedSquareFeets = contract.occupiedSquareFeets || 0;
    this.unoccupiedSquareFeets = contract.unoccupiedSquareFeets || 0;
    this.updatedDate = contract.updatedDate || new Date();

    this.customerCode = contract.customerCode || '';
    this.buildingCode = contract.buildingCode || '';
    this.monthlyProfit = contract.monthlyProfit || 0;
    this.MonthlyProfitRatio = contract.MonthlyProfitRatio || 0;
    this.totalMonthlyRevenue = contract.totalMonthlyRevenue || 0;
    this.totalMonthlyExpense = contract.totalMonthlyExpense || 0;
    this.totalMonthlyLaborExpense = contract.totalMonthlyLaborExpense || 0;
  }
}
