import { CompanyEntity } from '@core/models/common/company-entity.model';

export class BuildingGridModel extends CompanyEntity {

  name: string;
  fullAddress: string;
  operationsManagerFullName: string;
  isAvailable: number;
  isActive: number;
  isComplete: number;
  emergencyPhone: string;
  code: string;
  customerCode: string;
  customerId: number;

  // Statuses Representation

  get activeStatus(): string {
    return this.isActive ? 'Active' : 'Not Active';
  }

  get completeStatus(): string {
    return this.isComplete ? 'Yes' : 'Not';
  }

  get availableStatus(): string {
    return this.isAvailable ? 'Yes' : 'No';
  }

  constructor(building: BuildingGridModel) {
    super(building);
    this.name = building.name || '';
    this.fullAddress = building.fullAddress || '';
    this.operationsManagerFullName = building.operationsManagerFullName || '';
    this.isAvailable = building.isAvailable || 0;
    this.isActive = building.isActive || 0;
    this.isComplete = building.isComplete || 0;
    this.emergencyPhone = building.emergencyPhone || '';
    this.code = building.code || building.id.toString();
    this.customerCode = building.customerCode || '';
    this.customerId = building.customerId || 0;
  }
}
