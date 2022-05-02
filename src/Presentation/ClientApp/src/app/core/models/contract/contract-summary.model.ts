import { EntityModel } from '../common/entity.model';

export class ContractSummaryModel extends EntityModel {

  contractNumber: string;
  description: string;
  status: number;
  totalEstimatedRevenue: number;
  totalEstimatedExpenses: number;
  totalRealRevenue: number;
  totalRealExpenses: number;


  constructor(contractSummary: ContractSummaryModel) {
    super(contractSummary);

    this.contractNumber = contractSummary.contractNumber || '';
    this.description = contractSummary.description || '';
    this.status = contractSummary.status || 0;
    this.totalEstimatedRevenue = contractSummary.totalEstimatedRevenue || 0;
    this.totalEstimatedExpenses = contractSummary.totalEstimatedExpenses || 0;
    this.totalRealRevenue = contractSummary.totalRealRevenue || 0;
    this.totalRealExpenses = contractSummary.totalRealExpenses || 0;
  }
}
