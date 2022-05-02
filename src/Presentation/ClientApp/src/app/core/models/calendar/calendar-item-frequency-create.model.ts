import { CalendarItemFrequencyBaseModel } from '@app/core/models/calendar/calendar-item-frequency-base.model';

export class CalendarItemFrequencyCreateModel extends CalendarItemFrequencyBaseModel {
  workOrder: any;

  constructor(calendarItemFrequencyCreate: CalendarItemFrequencyCreateModel) {
    super(calendarItemFrequencyCreate);
  }
}
