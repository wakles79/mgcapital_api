import { CompanyEntity } from '../common/company-entity.model';
import { PreCalendarTaskModel } from './pre-calendar-task.model';

export class PreCalendarBaseModel extends CompanyEntity {

  snoozeDate: Date;
  description: string;
  type: number;
  buildingId: number;
  employeeId: number;
  quantity: number;
  periodicity: number;
  defineDate: boolean;
  tasks: PreCalendarTaskModel[];
  
  constructor(preCalendar: PreCalendarBaseModel) {
    super(preCalendar);

    this.snoozeDate = preCalendar.snoozeDate || null;
    this.description = preCalendar.description || '';
    this.type = preCalendar.type || 0;
    this.buildingId = preCalendar.buildingId || 0;
    this.employeeId = preCalendar.employeeId || 0;
    this.quantity = preCalendar.quantity || 0;
    this.periodicity = preCalendar.periodicity || 0;
    this.defineDate = preCalendar.defineDate || false;
    this.tasks = preCalendar.tasks || [];
  }
}
