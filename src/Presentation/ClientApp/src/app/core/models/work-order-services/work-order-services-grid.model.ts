import { WorkOrderServiceBaseModel } from '@app/core/models/work-order-services/work-order-services-base.model';

export class WorkOrderServiceGridModel extends WorkOrderServiceBaseModel {
  categoryName: string;
  frequencyName: string;

  constructor(servicesGrid) {
    super(servicesGrid);

    this.categoryName = servicesGrid.categoryName || '';
    this.frequencyName = servicesGrid.frequencyName || '';
  }
}
