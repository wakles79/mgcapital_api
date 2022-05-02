import { EntityModel } from '../common/entity.model';

export class CalendarItemModel extends EntityModel {

  snoozeDate: Date;
  description: string;
  type: number;
  epochSnoozeDate: number;

  constructor(calendarItem: CalendarItemModel) {
    super(calendarItem);

    this.snoozeDate = calendarItem.snoozeDate || null;
    this.description = calendarItem.description || '';
    this.type = calendarItem.type || 0;
    this.epochSnoozeDate = calendarItem.epochSnoozeDate || 0;
  }
}
