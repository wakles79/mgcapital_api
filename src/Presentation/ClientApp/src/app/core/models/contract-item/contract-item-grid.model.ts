import { ContractItemBaseModel } from './contract-item-base.model';

export class ContractItemGridModel extends ContractItemBaseModel {

  /** 1 Day */
  dailyRate: number;
  /** 1 Month */
  monthlyRate: number;
  /** 2 Months */
  biMonthlyRate: number;
  /** 3 Months */
  quarterly: number;
  /** 6 Months */
  biAnnually: number;
  /** 1 Year */
  yearlyRate: number;

  dailyRateFormula: string;
  monthlyRateFormula: string;
  biMonthlyRateFormula: string;
  quarterlyFormula: string;
  biAnnuallyFormula: string;
  yearlyRateFormula: string;
  order: number;

  updatePrepopulatedItems: boolean;

  constructor(contractItemGrid: ContractItemGridModel) {
    super(contractItemGrid);

    this.dailyRate = contractItemGrid.dailyRate || 0;
    this.monthlyRate = contractItemGrid.monthlyRate || 0;
    this.yearlyRate = contractItemGrid.yearlyRate || 0;
    this.order = contractItemGrid.order || 0;

    this.updatePrepopulatedItems = contractItemGrid.updatePrepopulatedItems || false;
  }
}
