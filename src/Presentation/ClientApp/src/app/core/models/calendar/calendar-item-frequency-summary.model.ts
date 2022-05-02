import { CalendarItemFrequencyBaseModel } from '@app/core/models/calendar/calendar-item-frequency-base.model';

export class CalendarItemFrequencySummaryModel extends CalendarItemFrequencyBaseModel {

  addedDates: Date[];

  constructor(calendarItemFrequencySummary: CalendarItemFrequencySummaryModel) {
    super(calendarItemFrequencySummary);

    this.addedDates = calendarItemFrequencySummary.addedDates || [];
  }

}
