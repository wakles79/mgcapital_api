import { EntityModel } from '../common/entity.model';

export class WorkOrderScheduleSetting extends EntityModel {

  workOrderId: number;
  frequency: number;
  startDate: Date;
  endDate: Date;
  ordinal: number;
  startValue: number;
  endValue: number;
  days: number[];
  scheduleDate: Date;
  excludedScheduleDates: Date[];

  constructor(setting: WorkOrderScheduleSetting) {
    super(setting);

    this.workOrderId = setting.workOrderId || 0;
    this.frequency = setting.frequency || 0;
    this.startDate = setting.startDate || null;
    this.endDate = setting.endDate || null;
    this.ordinal = setting.ordinal || null;
    this.startValue = setting.startValue || null;
    this.endValue = setting.endValue || null;
    this.days = setting.days || [];
    this.scheduleDate = setting.scheduleDate || null;
    this.excludedScheduleDates = setting.excludedScheduleDates || [];
  }
}
