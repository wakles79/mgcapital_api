import { EntityModel } from '@app/core/models/common/entity.model';
import { WorkOrderServiceFrequency } from '../work-order-services/work-order-services-base.model';

export class WorkOrderTaskCreateModel extends EntityModel {

  description: string;
  isComplete: boolean;
  serviceId: number;
  unitPrice: number;
  quantity: number;
  discountPercentage: number;
  note: string;
  workOrderServiceCategoryId: number;
  workOrderServiceId: number;
  unitFactor: number;
  frequency: WorkOrderServiceFrequency;
  rate: number;
  total: number;
  serviceName: string;
  location: string;
  attachments: any[];

  quantityExecuted: number;
  hoursExecuted: number;
  quantityRequiredAtClose: boolean;
  generalNote: string;

  constructor(create: WorkOrderTaskCreateModel) {
    super(create);
    this.description = create.description || '';
    this.isComplete = create.isComplete || false;
    this.serviceId = create.serviceId || 0;
    this.unitPrice = create.unitPrice || 0;
    this.quantity = create.quantity || 0;
    this.discountPercentage = create.discountPercentage || 0;
    this.note = create.note || '';
    this.workOrderServiceCategoryId = create.workOrderServiceCategoryId || 0;
    this.workOrderServiceId = create.workOrderServiceId || 0;
    this.unitFactor = create.unitFactor || 0;
    this.frequency = create.frequency || 0;
    this.rate = create.rate || 0;
    this.total = create.total || 0;
    this.serviceName = create.serviceName || '';
    this.location = create.location || '';
    this.attachments = create.attachments || [];
    this.quantityExecuted = create.quantityExecuted || 0;
    this.hoursExecuted = create.hoursExecuted || 0;
    this.quantityRequiredAtClose = create.quantityRequiredAtClose || false;
    this.generalNote = create.generalNote || '';
  }
}
