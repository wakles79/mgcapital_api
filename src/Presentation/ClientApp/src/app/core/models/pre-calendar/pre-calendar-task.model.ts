import { CompanyEntity } from '@core/models/common/company-entity.model';

export class PreCalendarTaskModel extends CompanyEntity {
  description: string;
  preCalendarId: number;
  isComplete: boolean;
  serviceId: number;
  serviceName: string;
  unitPrice: number;
  unitFactor: string;
  quantity: number;
  discountPercentage: number;
  note: string;

  createdDate: Date;
  epochCreatedDate: number;
  lastCheckedDate: Date;
  echoLastCheckedDate: number;

  // Computed properties
  get subTotal() {
    return (this.unitPrice || 0) * (this.quantity || 0);
  }
  get discount() {
    return this.subTotal * (this.discountPercentage || 0 / 100);
  }
  get total() {
    return (this.unitPrice || 0) * (this.quantity || 0) * (1 - (this.discountPercentage || 0) / 100);
  }

  constructor(pcTask: PreCalendarTaskModel) {
    super(pcTask);
    this.description = pcTask.description || '';
    this.isComplete = pcTask.isComplete || false;
    this.unitPrice = pcTask.unitPrice || 0;
    this.unitFactor = pcTask.unitFactor || '';
    this.serviceName = pcTask.serviceName || '';
    this.quantity = pcTask.quantity || 0;
    this.discountPercentage = pcTask.discountPercentage || 0;
    this.note = pcTask.note || '';

    this.createdDate = pcTask.createdDate || null;
    this.epochCreatedDate = pcTask.epochCreatedDate || 0;
    this.lastCheckedDate = pcTask.lastCheckedDate || null;
    this.echoLastCheckedDate = pcTask.echoLastCheckedDate || 0;

    // Relational fields
    this.preCalendarId = pcTask.preCalendarId || 0;
    this.serviceId = pcTask.serviceId || 0;


  }
}
