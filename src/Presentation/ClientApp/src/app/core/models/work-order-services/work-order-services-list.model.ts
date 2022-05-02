import { ListItemModel } from '@app/core/models/common/list-item.model';
import { WorkOrderServiceFrequency } from './work-order-services-base.model';

export class WorkOrderServiceListItemModel extends ListItemModel {

  unitFactor: string;
  frequency: WorkOrderServiceFrequency;
  rate: number;

  requiresScheduling: boolean;
  quantityRequiredAtClose: boolean;
  hoursRequiredAtClose: boolean;

  frequencyName: string;

  constructor(service) {
    super();

    this.unitFactor = service.unitFactor || '';
    this.frequency = service.frequency || WorkOrderServiceFrequency.OneTime;
    this.rate = service.rate || 0;

    this.frequencyName = service.frequencyName || '';
    this.requiresScheduling = service.requiresScheduling || false;
    this.quantityRequiredAtClose = service.quantityRequiredAtClose || false;
    this.hoursRequiredAtClose = service.hoursRequiredAtClose || false;
  }
}
