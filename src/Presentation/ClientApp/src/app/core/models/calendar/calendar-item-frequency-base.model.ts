import { EntityModel } from '@app/core/models/common/entity.model';
import { CALENDAR_FREQUENCY } from './calendar-periodicity-enum';

export class CalendarItemFrequencyBaseModel extends EntityModel {
  itemType: number;
  quantity: number;
  frequency: CALENDAR_FREQUENCY;
  startDate: Date;
  months: number[];

  constructor(calendarItemFrequency: CalendarItemFrequencyBaseModel) {
    super(calendarItemFrequency);

    this.itemType = calendarItemFrequency.itemType || 0;
    this.quantity = calendarItemFrequency.quantity || 1;
    this.frequency = calendarItemFrequency.frequency || 0;
    this.startDate = calendarItemFrequency.startDate || new Date();
    this.months = calendarItemFrequency.months || [];
  }
}
