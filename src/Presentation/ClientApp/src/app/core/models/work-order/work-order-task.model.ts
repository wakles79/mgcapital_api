import { CompanyEntity } from '@core/models/common/company-entity.model';
import { WorkOrderServiceFrequency } from '../work-order-services/work-order-services-base.model';

export class WorkOrderTaskModel extends CompanyEntity {

  workOrderId: number;
  isComplete: boolean;
  description: string;

  serviceId: number;
  unitPrice: number;
  quantity: number;
  discountPercentage: number;

  note: string;
  lastCheckedDate: Date;
  echoLastCheckedDate: number;
  location: string;

  workOrderServiceCategoryId: number;
  workOrderServiceId: number;
  unitFactor: string;
  frequency: WorkOrderServiceFrequency;
  rate: number;

  serviceName: string;

  createdDate: Date;
  epochCreatedDate: number;

  // Computed properties
  get subTotal(): number {
    return (this.unitPrice || 0) * (this.quantity || 0);
  }
  get discount(): number {
    return this.subTotal * (this.discountPercentage || 0 / 100);
  }
  get total(): number {
    return (this.unitPrice || 0) * (this.quantity || 0) * (1 - (this.discountPercentage || 0) / 100);
  }

  quantityExecuted: number;
  hoursExecuted: number;
  completedDate: Date;
  quantityRequiredAtClose: boolean;
  generalNote: string;


  constructor(woTask: WorkOrderTaskModel) {
    super(woTask);
    this.description = woTask.description || '';
    this.isComplete = woTask.isComplete || false;
    this.unitPrice = woTask.unitPrice || 0;
    this.unitFactor = woTask.unitFactor || '';
    this.serviceName = woTask.serviceName || '';
    this.quantity = woTask.quantity || 0;
    this.discountPercentage = woTask.discountPercentage || 0;
    this.note = woTask.note || '';

    this.createdDate = woTask.createdDate || null;
    this.epochCreatedDate = woTask.epochCreatedDate || 0;
    this.lastCheckedDate = woTask.lastCheckedDate || null;
    this.echoLastCheckedDate = woTask.echoLastCheckedDate || 0;

    // Relational fields
    this.workOrderId = woTask.workOrderId || 0;
    this.serviceId = woTask.serviceId || 0;

    this.location = woTask.location || '';
    this.workOrderServiceCategoryId = woTask.workOrderServiceCategoryId || null;
    this.workOrderServiceId = woTask.workOrderServiceId || null;
    this.unitFactor = woTask.unitFactor || '';
    this.frequency = woTask.frequency || WorkOrderServiceFrequency.OneTime;
    this.rate = woTask.rate || 0;

    this.quantityExecuted = woTask.quantityExecuted || 0;
    this.hoursExecuted = woTask.hoursExecuted || 0;
    this.completedDate = woTask.completedDate || null;
    this.quantityRequiredAtClose = woTask.quantityRequiredAtClose || false;
    this.generalNote = woTask.generalNote || '';
  }
}
