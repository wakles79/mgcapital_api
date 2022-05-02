import { PreCalendarBaseModel } from './pre-calendar-base.model';

export class PreCalendarGridModel extends PreCalendarBaseModel {

  typeName: string;
  periodicityName: string;
  buildingName: string;
  employeeName: string; 
  status: string;

  constructor(preCalendarGridModel: PreCalendarGridModel) {
    super(preCalendarGridModel);
    
    this.typeName = preCalendarGridModel.typeName || '';
    this.periodicityName = preCalendarGridModel.periodicityName || '';
    this.buildingName = preCalendarGridModel.buildingName || '';
    this.employeeName = preCalendarGridModel.employeeName || '';
  }
}
