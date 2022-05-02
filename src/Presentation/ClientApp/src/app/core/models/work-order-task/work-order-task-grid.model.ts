import { EntityModel } from '../common/entity.model';

export class WorkOrderTaskGridModel extends EntityModel {

  location: string;
  description: string;
  quantity: number;
  rate: number;
  unitFactor: string;
  serviceId: number;
  workOrderServiceId: number;
  serviceName: string;
  categoryName: string;
  isComplete: boolean;
  total: number;
  oldVersion: boolean;

  requiresScheduling: boolean;
  quantityRequiredAtClose: boolean;
  hoursRequiredAtClose: boolean;

  quantityExecuted: number;
  hoursExecuted: number;
  completedDate: Date;
  epochCompletedDate: number;

  frequencyName: string;

  constructor(grid) {
    super(grid);
    this.location = grid.location || '';
    this.description = grid.description || '';
    this.quantity = grid.quantity || 0;
    this.rate = grid.rate || 0;
    this.unitFactor = grid.unitFactor || '';
    this.serviceName = grid.serviceName || '';
    this.categoryName = grid.categoryName || '';
    this.isComplete = grid.isComplete || false;
    this.total = grid.total || 0;
    this.oldVersion = grid.oldVersion || false;

    this.requiresScheduling = grid.requiresScheduling || false;
    this.quantityRequiredAtClose = grid.quantityRequiredAtClose || false;
    this.hoursRequiredAtClose = grid.hoursRequiredAtClose || false;

    this.quantityExecuted = grid.quantityExecuted || 0;
    this.hoursExecuted = grid.hoursExecuted || 0;

    this.serviceId = grid.serviceId || 0;
    this.workOrderServiceId = grid.workOrderServiceId || 0;
    this.frequencyName = grid.frequencyName || '';
    this.completedDate = grid.completedDate || null;
    this.epochCompletedDate = grid.epochCompletedDate || 0;
  }
}
