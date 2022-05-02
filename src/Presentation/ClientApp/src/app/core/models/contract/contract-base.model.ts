import { CompanyEntity } from '@core/models/common/company-entity.model';
import { ContractOfficeSpaceModel } from './contract-office-space.model';

export class ContractBaseModel extends CompanyEntity {
  contractNumber: string;
  area: number;
  numberOfPeople: number;
  buildingId: number;
  customerId: number;
  contactSignerId: number;
  description: string;
  daysPerMonth: number;
  numberOfRestrooms: number;
  frequencyPerYear: number;
  expirationDate: number;
  status: number;

  productionRate: number;
  unoccupiedSquareFeets: number;
  officeSpaces: ContractOfficeSpaceModel[];
  editionCompleted: boolean;

  dailyProfit: number;
  monthlyProfit: number;
  yearlyProfit: number;
  dailyProfitRatio: number;
  monthlyProfitRatio: number;
  yearlyProfitRatio: number;

  updatePrepopulatedItems: boolean;

  constructor(contract: ContractBaseModel) {
    super(contract);
    this.contractNumber = contract.contractNumber || '';
    this.area = contract.area || 0;
    this.numberOfPeople = contract.numberOfPeople || 0;
    this.buildingId = contract.buildingId || null;
    this.customerId = contract.customerId || null;
    this.contactSignerId = contract.contactSignerId || 0;
    this.description = contract.description;
    this.daysPerMonth = contract.daysPerMonth || 0;
    this.numberOfRestrooms = contract.numberOfRestrooms || 0;
    this.frequencyPerYear = contract.frequencyPerYear || 0;
    this.expirationDate = contract.expirationDate || null;
    this.status = contract.status || 0;

    this.productionRate = contract.productionRate || 0;
    this.unoccupiedSquareFeets = contract.unoccupiedSquareFeets || 0;
    this.officeSpaces = contract.officeSpaces || [];
    this.editionCompleted = contract.editionCompleted || false;

    this.dailyProfit = contract.dailyProfit || 0;
    this.monthlyProfit = contract.monthlyProfit || 0;
    this.yearlyProfit = contract.yearlyProfit || 0;
    this.dailyProfitRatio = contract.dailyProfitRatio || 0;
    this.monthlyProfitRatio = contract.monthlyProfitRatio || 0;
    this.yearlyProfitRatio = contract.yearlyProfitRatio || 0;

    this.updatePrepopulatedItems = contract.updatePrepopulatedItems || false;
  }
}
