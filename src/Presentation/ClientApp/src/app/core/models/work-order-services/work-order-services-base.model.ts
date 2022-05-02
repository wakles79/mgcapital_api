import { EntityModel } from '@app/core/models/common/entity.model';

export class WorkOrderServiceBaseModel extends EntityModel {
  workOrderServiceCategoryId: number;
  name: string;
  unitFactor: string;
  frequency: WorkOrderServiceFrequency;
  rate: number;

  requiresScheduling: boolean;
  quantityRequiredAtClose: boolean;
  hoursRequiredAtClose: boolean;

  constructor(service) {
    super(service);

    this.workOrderServiceCategoryId = service.workOrderServiceCategoryId || 0;
    this.name = service.name || '';
    this.unitFactor = service.unitFactor || '';
    this.frequency = service.frequency || WorkOrderServiceFrequency.Daily;
    this.rate = service.rate || 0;

    this.requiresScheduling = service.requiresScheduling || false;
    this.quantityRequiredAtClose = service.quantityRequiredAtClose || false;
    this.hoursRequiredAtClose = service.hoursRequiredAtClose || false;
  }
}

export enum WorkOrderServiceFrequency {
  Daily,
  Weekly,
  Monthly,
  TwiceAMonth,
  BiMonthly,
  Quarterly,
  SemiAnnually,
  Anually,
  OneTime
}
