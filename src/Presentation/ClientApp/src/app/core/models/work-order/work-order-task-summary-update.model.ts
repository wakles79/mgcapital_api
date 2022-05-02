import { EntityModel } from '@app/core/models/common/entity.model';

export class WorkOderTaskSummaryUpdateModel extends EntityModel {

  clientApproved: boolean;
  scheduleDateConfirmed: boolean;
  scheduleDate: Date;

  tasks: WorkOrderTaskCalendarUpdateModel[];

  constructor(workOderTaskSummaryUpdate: WorkOderTaskSummaryUpdateModel) {
    super(workOderTaskSummaryUpdate);

    this.clientApproved = workOderTaskSummaryUpdate.clientApproved || false;
    this.scheduleDateConfirmed = workOderTaskSummaryUpdate.scheduleDateConfirmed || false;
    this.scheduleDate = workOderTaskSummaryUpdate.scheduleDate || new Date();

    this.tasks = workOderTaskSummaryUpdate.tasks || [];
  }
}

export class WorkOrderTaskCalendarUpdateModel extends EntityModel {
  description: string;
  unitPrice: number;

  constructor(workOrderTaskCalendarUpdate: WorkOrderTaskCalendarUpdateModel) {
    super(workOrderTaskCalendarUpdate);

    this.description = workOrderTaskCalendarUpdate.description || '';
    this.unitPrice = workOrderTaskCalendarUpdate.unitPrice || 0;
  }
}
